namespace WremiaTrade.Services.Abstraction
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    using Papara.Models.Entities;
    using Papara.Services.DTO;
    using Papara.Services.DTO.Notification;
    using Papara.Services.Interface;
    using Papara.Services.Monitoring;
    using Papara.Services.Resources;
    using Papara.Services.UserDevice.DTO;
    using Papara.Utilities;
    using Papara.Utilities.Extensions;

    using Environment = System.Environment;

    public class NotificationPackageBuilder
    {
        private static readonly IConfigurationService _configurationService = new ConfigurationService();

        public static IResourceReadService ResourcesService { get; set; }

        public static IStringAdapter StringAdapter { get; set; }

        private static string ResourceValue(ResourceInfo resourceInfo, string language)
        {
            return ResourcesService.GetResource<string>(resourceInfo.Key, language);
        }
        
        private static string ResourceValue(ResourceInfo resourceInfo)
        {
            return ResourcesService.GetResource<string>(resourceInfo.Key);
        }

        #region EmailNotificationPackages

        public static EmailNotificationPackage AddRoleToTheUser(string userId, string rolename, string merchantId, AuditLogOperationName operationName)
        {
            var subject = "Yetki ataması";

            var contentBasedOnOperation = operationName == AuditLogOperationName.AdminAddedRoleEndUser ? "admin" :
                operationName == AuditLogOperationName.AdminAddedRoleMerchantUser ? $"merchant ({merchantId})" : "";

            var content = $"{userId} idli {contentBasedOnOperation} kullanıcısına {rolename} yetkisi atanmıştır.";

            _configurationService.ReadConfiguration("AccessRightNotificationTo", out string toEmail);

            return EmailTemplates.DefaultMailPackage(string.Empty, content, subject, new List<string> { toEmail }, string.Empty);
        }

        /// <summary>
        /// Edu Papara Card işlemlerinde hata olduğunu bilgilendirme maili
        /// </summary>
        public static EmailNotificationPackage MifareNotification(List<string> errors)
        {
            var content = string.Join(Environment.NewLine, errors.ToArray());

            var subject = "Edu card işlemede hata oluştu";

            var to = "it@papara.com";

            return new EmailNotificationPackage(string.Empty, content, subject, new List<string> { to });
        }

        /// <summary>
        /// Iban doğrulama bildirimi
        /// </summary>
        public static EmailNotificationPackage VerifyIbanNotification(int remainingCount)
        {
            var subject = "KKB IBAN Doğrulama Kalan Limit";

            var content = $"KKB servisinde IBAN doğrulama kalan kullanım belirtilen limitin altına düşmüştür. Kalan limit: {remainingCount}";

            var to = "operasyon@papara.com; muhasebe@papara.com";

            return new EmailNotificationPackage(string.Empty, content, subject, new List<string> { to });
        }

        /// <summary>
        /// Üye işyerine kullanıcı ekleme bilgilendirme maili
        /// </summary>
        public static EmailNotificationPackage CreateMerchantNotification(Merchant merchant, User user)
        {
            if (user.EmailConfirmed)
            {
                return EmailTemplates.GetCreateMerchantMailPackage(user, merchant, "Üye İşyeri Portalına eklendiniz", new List<string> { user.Email });
            }

            return null;
        }

        /// <summary>
        /// Send notifications based on updates we receive from our card delivery company to the Card.Delivery.API.
        /// </summary>
        public static EmailNotificationPackage SendCardDeliveryStatusChangeNotifications(string fullName, string subject, string content, string email)
        {
            return EmailTemplates.PaparaCardStatusMailPackage(fullName,
                content,
                subject,
                new List<string> { email });
        }

        /// <summary>
        /// Para çekeme işlemi için operasyona işlemin banka tarafından yapılmamış olabileceğibi bilgilendiren mail atar.
        /// </summary>
        public static EmailNotificationPackage WithdrawalWarnNotification(BankTransferRequest request, string companyBankName, string bankMessage, string bankReferenceCode)
        {
            _configurationService.ReadConfiguration("PaparaWebUrl", out string url, false);

            _configurationService.ReadConfiguration("MailTo", out string to, false);

            var currencyInfo = CurrencyService.GetCurrencyInfo(request.Currency);

            return EmailTemplates.GetAutobankInformMailPackage(request.Id
                , request.UserId
                , request.User.FullName
                , request.UserBankAccount.Bank.ShortName
                , request.UserBankAccount.Iban
                , Math.Abs(request.Amount)
                , currencyInfo.PreferredDisplayCode
                , companyBankName
                , bankMessage
                , bankReferenceCode
                , url
                , request.User.AccountNumber.ToString()
                , $"Banka transferi yapılamadı. {request.User.FullName}"
                , new List<string> { to }
                , request.User.DefaultLanguage);
        }

        /// <summary>
        /// Üye işyeri Para çekeme işlemi için operasyona işlemin banka tarafından yapılmamış olabileceğibi bilgilendiren mail atar.
        /// </summary>
        public static EmailNotificationPackage MerchantWithdrawalWarnNotification(MerchantBankTransferRequest request, string companyBankName, string bankMessage, string bankReferenceCode)
        {
            _configurationService.ReadConfiguration("PaparaWebUrl", out string url, false);

            _configurationService.ReadConfiguration("MailTo", out string to, false);

            var currencyInfo = CurrencyService.GetCurrencyInfo(request.Currency);

            return EmailTemplates.GetAutobankInformMailPackage(request.Id
                , request.MerchantId
                , request.Merchant.LegalName
                , request.MerchantBankAccount.Bank.ShortName
                , request.MerchantBankAccount.Iban
                , Math.Abs(request.Amount)
                , currencyInfo.PreferredDisplayCode
                , companyBankName
                , bankMessage
                , bankReferenceCode
                , url
                , request.Merchant.MerchantNumber.ToString()
                , $"Banka transferi yapılamadı. {request.Merchant.LegalName}"
                , new List<string> { to }
                , string.Empty);
        }

        /// <summary>
        /// Para çekeme işlemi için operasyona kontrol edilmesi gerektiğini bildiren maili
        /// </summary>
        public static EmailNotificationPackage WithdrawalControlNotification(BankTransferRequest request, string companyBankName)
        {
            _configurationService.ReadConfiguration("PaparaWebUrl", out string url, false);

            _configurationService.ReadConfiguration("MailTo", out string to, false);

            var currencyInfo = CurrencyService.GetCurrencyInfo(request.Currency);

            return EmailTemplates.GetAutobankControlMailPackage(request.Id
                , request.UserId
                , request.User.FullName
                , request.UserBankAccount.Bank.ShortName
                , request.UserBankAccount.Iban
                , Math.Abs(request.Amount)
                , currencyInfo.PreferredDisplayCode
                , companyBankName
                , url
                , request.User.AccountNumber
                , $"Banka transferi yapılamadı. {request.User.FullName}"
                , new List<string> { to }
                , request.User.DefaultLanguage);
        }

        /// <summary>
        /// Para çekeme işlemi için operasyona kontrol edilmesi gerektiğini bildiren maili
        /// </summary>
        public static EmailNotificationPackage MerchantWithdrawalControlNotification(MerchantBankTransferRequest request, string companyBankName)
        {
            _configurationService.ReadConfiguration("PaparaWebUrl", out string url, false);

            _configurationService.ReadConfiguration("MailTo", out string to, false);

            var currencyInfo = CurrencyService.GetCurrencyInfo(request.Currency);

            return EmailTemplates.GetAutobankControlMailPackage(request.Id
                , request.MerchantId
                , request.Merchant.LegalName
                , request.MerchantBankAccount.Bank.ShortName
                , request.MerchantBankAccount.Iban
                , Math.Abs(request.Amount)
                , currencyInfo.PreferredDisplayCode
                , companyBankName
                , url
                , request.Merchant.MerchantNumber
                , $"Banka transferi yapılamadı. {request.Merchant.LegalName}"
                , new List<string> { to }
                , string.Empty);
        }

        /// <summary>
        /// Kullanıcının eposta adresini onaylamasını bildiren maili
        /// </summary>
        public static EmailNotificationPackage ConfirmationEmail(User user, string disConfirmationToken, string confirmationCode)
        {
            _configurationService.ReadConfiguration("PaparaWebUrl", out string url, false);

            var disConfirmUrl = url + "personal/#!/emaildisconfirmation?";

            url += "personal/#!/emailconfirmation/";

            var encodedEmail = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Email));

            var mailPackageModel = new SignupConfirmationMailModel
            {
                Subject = ResourceValue(ResourceConstants.ConfirmationMailTitle, user.DefaultLanguage),
                To = new List<string> { user.Email },
                Url = url,
                DisConfirmUrl = disConfirmUrl,
                UserId = user.Id + "&",
                FullName = user.FullName,
                DisconfirmationToken = disConfirmationToken + "&",
                EmailConfirmationCode = confirmationCode + "&",
                Email = encodedEmail,
                Language = user.DefaultLanguage
            };

            var package = EmailTemplates.GetSignupConfirmationMailPackage(mailPackageModel);

            return package;
        }

        /// <summary>
        /// Sends a welcome mail to user if X days passed after the register date
        /// </summary>
        public static EmailNotificationPackage WelcomeMail(User user)
        {
            var mailSubject = ResourceValue(ResourceConstants.WelcomeEmailTitle, user.DefaultLanguage);

            _configurationService.ReadConfiguration("PaparaWebUrl", out string url, false);

            var package = EmailTemplates.GetWelcomeMail(url, user.FullName, mailSubject, new List<string> { user.Email }, user.DefaultLanguage);

            return package;
        }

        /// <summary>
        /// Kullanıcının eposta değişimini bildiren maili
        /// </summary>
        public static EmailNotificationPackage ChangeEmailCodeNotification(string email, string confirmationCode, string language)
        {
            var mailSubject = ResourceValue(ResourceConstants.EmailChangeCodeEmailTitle, language);

            return EmailTemplates.ChangeEmailMailPackage(confirmationCode, mailSubject, new List<string> { email }, language);
        }

        /// <summary>
        /// Kullanıcının kampnaya izinlerini bildiren maili
        /// </summary>
        public static EmailNotificationPackage ManageNotification(User user, string status)
        {
            var mailSubject = ResourceValue(ResourceConstants.CampaignNotificationsEmailTitle, user.DefaultLanguage);

            return EmailTemplates.NotificationEmailPackage(user.FirstName, user.LastName, status, mailSubject, new List<string> { user.Email }, user.DefaultLanguage);
        }

        /// <summary>
        /// Kullanıcı şifremi unuttum bilgilendirme maili
        /// </summary>
        public static EmailNotificationPackage ForgotPasswordNotification(string userId, string passwordResetToken, string toEmail, string url, string language)
        {
            return EmailTemplates.GetForgotPasswordMailPackage(url, userId + "&", passwordResetToken, ResourceValue(ResourceConstants.ForgotPasswordEmailSubject), new List<string> { toEmail }, language);
        }

        /// <summary>
        /// Papara Card gün sonu iade işlemlerinin bilgilendirme maili
        /// </summary>
        public static EmailNotificationPackage CardRefundErrorNotification(string fileName, int unsuccessfullCount, string errors)
        {
            var subject = "İade edilemeyen kayıtlar var.";

            return EmailTemplates.GetPaparaCardRefundFailedMailPackage(fileName, unsuccessfullCount.ToString(), errors,
                subject, new List<string> { "kartoperasyon@papara.com" });

        }

        /// <summary>
        /// Kullanıcı şifremi unuttum bilgilendirme maili
        /// </summary>
        public static EmailNotificationPackage GetTicketInfoNotification(string userFullName, string ticketMessage, string ticketTitle, string toEmail, string language)
        {
            var mailSubject = ResourceValue(ResourceConstants.GetTicketInfoEmailTitle, language);

            return EmailTemplates.GetTicketInfoMailPackage(userFullName, ticketMessage, ticketTitle, mailSubject, new List<string> { toEmail }, language);
        }

        /// <summary>
        /// Boş mail templete ile mail oluşturur
        /// </summary>
        public static EmailNotificationPackage CustomEmail(string subject, string content, string email)
        {
            return EmailTemplates.EmptyMailPackage(content, subject, email.Split(',').ToList());
        }

        /// <summary>
        /// Build qr code generated document mail notification
        /// </summary>
        public static EmailNotificationPackage GeneratedQRCodeNotification(string language, List<string> to, byte[] documentBytes, int documentCount)
        {
            var subject = StringAdapter.Format(ResourceValue(ResourceConstants.QrCodesGeneratedSubject, language), documentCount);

            var content = StringAdapter.Format(ResourceValue(ResourceConstants.QrCodesGeneratedContent, language), documentCount);

            var documentName = $"qr_codes_{documentCount}_{DateTime.Now:yyyy.MM.dd hh:mm:ss}.zip";

            return EmailTemplates.GetGeneratedQRDocumentsMailPackage(language, content, subject, to, documentBytes, documentName);
        }

        /// <summary>
        /// Şireket banka hesabına transfer işlemi sonucu bilgilendirmesi
        /// </summary>
        public static EmailNotificationPackage CompanyBankAccountBalanceTransferNotification(ServiceResult serviceResult, BalanceTransferModel model)
        {
            var subject = "Banka Transferi ";

            var result = "BAŞARISIZ";

            if (serviceResult.Succeeded)
            {
                result = "BAŞARILI";
            }

            subject += result;

            var content = $"{model.BalanceTransferIban} ibanına {model.Amount} {model.BalanceTransferCurrency} transfer isteği {result} olmuştur. { serviceResult.Error?.Message}";

            _configurationService.ReadConfiguration("CompanyBankAccountBalanceTransferNotificationTo", out string toEmail);

            return EmailTemplates.EmptyMailPackage(content, subject, new List<string> { toEmail });
        }

        #endregion

        #region SmsNotificationPackages

        /// <summary>
        /// Forgot password sms notification
        /// </summary>
        public static SmsNotificationPackage ForgotPasswordSMSNotification(string phoneNumber, string url, string language)
        {
            var content = string.Format(ResourceValue(ResourceConstants.ForgotPasswordSmsMessage, language), url);

            return new SmsNotificationPackage(content, phoneNumber);
        }


        /// <summary>
        /// Kullanıcı Telefon numarası değiştirme mesajı
        /// </summary>
        public static SmsNotificationPackage ChangePhoneNumberVerification(string smsCode, string phoneNumber)
        {
            var content = StringAdapter.Format(ResourceValue(ResourceConstants.SmsMessageForPhoneNumberChange), smsCode);

            return new SmsNotificationPackage(content, phoneNumber);
        }

        /// <summary>
        /// Kullanıcı Telefon numarası değiştirildi mesajı
        /// </summary>
        public static SmsNotificationPackage PhoneNumberChangedNotification(string phoneNumber)
        {
            var content = ResourceValue(ResourceConstants.PhoneNumberWasChangedByUser);

            return new SmsNotificationPackage(content, phoneNumber);
        }

        /// <summary>
        /// Kullanıcı Telefon numarası kaldırılma mesajı
        /// </summary>
        public static SmsNotificationPackage PhoneNumberRemovedNotification(string phoneNumber)
        {
            var content = ResourceValue(ResourceConstants.PhoneNumberWasRemovedByOperator);

            return new SmsNotificationPackage(content, phoneNumber);
        }

        /// <summary>
        /// Kullanıcı giriş mesajı
        /// </summary>
        public static SmsNotificationPackage LoginNotification(string smsCode, string phoneNumber)
        {
            var content = StringAdapter.Format(ResourceValue(ResourceConstants.LoginSms), smsCode);

            return new SmsNotificationPackage(content, phoneNumber);
        }

        /// <summary>
        /// Kullanıcı kayıt mesajı
        /// </summary>
        public static SmsNotificationPackage RegisterNotification(string smsCode, string phoneNumber)
        {
            var content = StringAdapter.Format(ResourceValue(ResourceConstants.RegisterSms), smsCode);

            return new SmsNotificationPackage(content, phoneNumber);
        }

        /// <summary>
        /// Kantin kurulum mesajı
        /// </summary>
        public static SmsNotificationPackage InstallCanteenNotification(long terminalId, string smsCode, string phoneNumber)
        {
            var content = StringAdapter.Format(ResourceValue(ResourceConstants.InstallTerminalSms), terminalId, smsCode);

            return new SmsNotificationPackage(content, phoneNumber);
        }

        /// <summary>
        /// Kurumsal Papara Card tanımlama mesajı
        /// </summary>
        public static SmsNotificationPackage CorporateCardDefinitionNotification(string smsCode, string phoneNumber)
        {
            var content = StringAdapter.Format(ResourceValue(ResourceConstants.CorporateCardDefinationSms), smsCode);

            return new SmsNotificationPackage(content, phoneNumber);
        }

        /// <summary>
        /// Papara card şifresi belirleme mesajı
        /// </summary>
        public static SmsNotificationPackage PaparaCardDeterminePinNotification(string phoneNumber, string cardPan, string pin)
        {
            var content = StringAdapter.Format(ResourceValue(ResourceConstants.PaparaCardDeterminePinNotification), cardPan, pin);

            return new SmsNotificationPackage(content, phoneNumber);
        }

        /// <summary>
        /// Harçlık talimatı para gönderme limitine takılıp gerçekleşmediği durumda gönderilecek mesaj
        /// </summary>
        public static SmsNotificationPackage PocketMoneyInstructionSendTransitionRangeLimitNotification(string phoneNumber)
        {
            return new SmsNotificationPackage(ResourceValue(ResourceConstants.PocketMoneyInstructionSendTransitionRangeLimitNotification), phoneNumber);
        }
        #endregion

        #region AllInOne

        public static NotificationPackage ApprovementUserDeviceNotifications(ApprovementUserDeviceNotificationsModel model)
        {
            string deviceDetail = model.Platform == Platform.WEB ?
                model.UserDevice.Browser :
                $"{model.UserDevice.MobileDeviceModel} {model.UserDevice.MobileDeviceDescription}";

            string content;

            if (model.Platform == Platform.WEB)
            {
                content = StringAdapter.Format(ResourceValue(ResourceConstants.UserDeviceApprovementContent, model.User.DefaultLanguage), model.UserDevice.Environment, model.UserDevice.OperatingSystem, deviceDetail, model.IpAddress);
            }
            else
            {
                content = StringAdapter.Format(ResourceValue(ResourceConstants.UserMobileDeviceApprovementContent, model.User.DefaultLanguage), model.UserDevice.Environment, model.UserDevice.OperatingSystem, deviceDetail, model.IpAddress);
            }

            _configurationService.ReadConfiguration("PaparaWebUrl", out string baseUrl, false);

            var baseUrlUri = new Uri(baseUrl);

            var verificationUrl = string.IsNullOrWhiteSpace(model.VerificationLink) ? $"{baseUrl}personal/#!/user/device/approve?deviceId={model.UserDeviceId}" : StringAdapter.Format(model.VerificationLink, baseUrl, model.UserDeviceId);

            var url = new Uri(baseUrlUri, verificationUrl);

            if (model.User.EmailConfirmed)
            {
                return EmailTemplates.UserDeviceApprovementMailPackage(model.User.FullName, content, ResourceValue(ResourceConstants.UserDeviceApprovementSubject, model.User.DefaultLanguage), url.ToString(), new List<string> { model.User.Email }, model.User.DefaultLanguage,model.MagicPixelUrl);
            }

            if (model.Platform == Platform.WEB)
            {
                content = StringAdapter.Format(ResourceValue(ResourceConstants.UserDeviceApprovementSmsContent, model.User.DefaultLanguage), model.UserDevice.Environment, model.UserDevice.OperatingSystem, deviceDetail, model.IpAddress, url.ToString());
            }
            else
            {
                content = StringAdapter.Format(ResourceValue(ResourceConstants.UserMobileDeviceApprovementSmsContent, model.User.DefaultLanguage), model.UserDevice.Environment, model.UserDevice.OperatingSystem, deviceDetail, model.IpAddress, url.ToString());
            }

            return new SmsNotificationPackage(content, model.User.PhoneNumber);
        }

        /// <summary>
        /// Harçlık gönderme bildirmleri
        /// </summary>
        public static IList<NotificationPackage> SuccessDefinedPocketMoney(User parent, User student, string period, decimal amount, string currencyCode)
        {
            var notificationPackages = new List<NotificationPackage>();

            string content, subject;

            // Notification
            if (parent.CanPushMobileNotification)
            {
                subject = ResourceValue(ResourceConstants.EduCompletedDefinePocketMoneyNotificationSubject, parent.DefaultLanguage);

                content = StringAdapter.Format(ResourceValue(ResourceConstants.EduCompletedDefinePocketMoneyNotificationContent, parent.DefaultLanguage), student.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName), period.ToLower(), amount, currencyCode);

                notificationPackages.Add(new PushNotificationPackage(content, subject, parent.DeviceEndpoint, parent.Id, parent.DeviceToken, parent.DeviceType));
            }

            // Email
            if (parent.EmailConfirmed)
            {
                subject = StringAdapter.Format(ResourceValue(ResourceConstants.EduCompletedDefinePocketMoneyMailSubject, parent.DefaultLanguage));

                _configurationService.ReadConfiguration("PaparaWebUrl", out string url, false);

                url += "personal/#!/pocket-money";

                var dynamicContent = StringAdapter.Format(ResourceValue(ResourceConstants.EduCompletedDefinePocketMoneyMailDynamicContent, parent.DefaultLanguage), student.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName), period.ToLower(), amount, currencyCode);

                var package = EmailTemplates.GetEduMailPackage(subject, parent.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName), dynamicContent, url, new List<string> { parent.Email }, parent.DefaultLanguage);

                notificationPackages.Add(package);
            }

            // SMS
            if (!parent.CanPushMobileNotification && !parent.EmailConfirmed && !string.IsNullOrWhiteSpace(parent.PhoneNumber))
            {
                content = StringAdapter.Format(ResourceValue(ResourceConstants.EduCompletedDefinePocketMoneySmsContent, parent.DefaultLanguage), student.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName), period.ToLower(), amount, currencyCode);

                notificationPackages.Add(new SmsNotificationPackage(content, parent.PhoneNumber));
            }

            return notificationPackages;
        }

        /// <summary>
        /// Öğrenci kaldırma bildirmleri
        /// </summary>
        public static IList<NotificationPackage> EduRemoveStudent(User parent, User student)
        {
            var notificationPackages = new List<NotificationPackage>();

            string content, subject;

            // Notification
            if (parent.CanPushMobileNotification)
            {
                subject = ResourceValue(ResourceConstants.EduRemoveStudentNotificationSubject, parent.DefaultLanguage);

                content = StringAdapter.Format(ResourceValue(ResourceConstants.EduRemoveStudentNotificationContent, parent.DefaultLanguage), parent.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName));

                notificationPackages.Add(new PushNotificationPackage(content, subject, student.DeviceEndpoint, student.Id, student.DeviceToken, student.DeviceType));
            }

            // Email
            if (student.EmailConfirmed)
            {
                subject = StringAdapter.Format(ResourceValue(ResourceConstants.EduRemoveStudentMailSubject, student.DefaultLanguage), parent.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName));

                _configurationService.ReadConfiguration("PaparaWebUrl", out string url, false);

                url += "personal/#!//edu-student";

                var dynamicContent = StringAdapter.Format(ResourceValue(ResourceConstants.EduRemoveStudentMailDynamicContent, student.DefaultLanguage), parent.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName));

                var package = EmailTemplates.GetEduMailPackage(subject, student.FullName, dynamicContent, url, new List<string> { student.Email }, student.DefaultLanguage);

                notificationPackages.Add(package);
            }

            // SMS
            if (!parent.CanPushMobileNotification && !student.EmailConfirmed && !string.IsNullOrWhiteSpace(student.PhoneNumber))
            {
                content = StringAdapter.Format(ResourceValue(ResourceConstants.EduRemoveStudentSmsContent, student.DefaultLanguage), parent.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName));

                notificationPackages.Add(new SmsNotificationPackage(content, student.PhoneNumber));
            }

            return notificationPackages;
        }

        /// <summary>
        /// Harçlık talimatı düzenleme bildirmleri
        /// </summary>
        public static IList<NotificationPackage> EduStudentUpdateDefinePocketMoney(User parent, User student, string period, decimal amount, string currencyCode)
        {
            var notificationPackages = new List<NotificationPackage>();

            string content, subject;

            // Notification
            if (student.CanPushMobileNotification)
            {
                subject = ResourceValue(ResourceConstants.EduStudentUpdatePocketMoneyNotificationSubject, student.DefaultLanguage);

                content = StringAdapter.Format(ResourceValue(ResourceConstants.EduStudentUpdatePocketMoneyNotificationContent, student.DefaultLanguage), parent.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName), period.ToLower(), amount, currencyCode);

                notificationPackages.Add(new PushNotificationPackage(content, subject, student.DeviceEndpoint, student.Id, student.DeviceToken, student.DeviceType));
            }

            // Email
            if (student.EmailConfirmed)
            {
                subject = StringAdapter.Format(ResourceValue(ResourceConstants.EduStudentUpdatePocketMoneyMailSubject, student.DefaultLanguage), parent.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName));

                _configurationService.ReadConfiguration("PaparaWebUrl", out string url, false);

                url += "personal/#!/edu-student";

                var dynamicContent = StringAdapter.Format(ResourceValue(ResourceConstants.EduStudentUpdatePocketMoneyMailDynamicContent, student.DefaultLanguage), parent.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName), period, amount, currencyCode);

                var package = EmailTemplates.GetEduMailPackage(subject, student.FullName, dynamicContent, url, new List<string> { student.Email }, student.DefaultLanguage);

                notificationPackages.Add(package);
            }

            // SMS
            if (!student.CanPushMobileNotification && !student.EmailConfirmed && !string.IsNullOrWhiteSpace(student.PhoneNumber))
            {
                content = StringAdapter.Format(ResourceValue(ResourceConstants.EduStudentUpdatePocketMoneySmsContent, student.DefaultLanguage), parent.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName), period.ToLower(), amount, currencyCode);

                notificationPackages.Add(new SmsNotificationPackage(content, student.PhoneNumber));
            }

            return notificationPackages;
        }

        /// <summary>
        /// Harçlık talimatı tanımlama bildirmleri
        /// </summary>
        public static IList<NotificationPackage> EduStudentDefinePocketMoney(User parent, User student, string period, decimal amount, string currencyCode)
        {
            var notificationPackages = new List<NotificationPackage>();

            string content, subject;

            // Notification
            if (student.CanPushMobileNotification)
            {
                subject = ResourceValue(ResourceConstants.EduStudentPocketMoneyNotificationSubject, student.DefaultLanguage);

                content = StringAdapter.Format(ResourceValue(ResourceConstants.EduStudentPocketMoneyNotificationContent, student.DefaultLanguage), parent.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName), period.ToLower(), amount, currencyCode);

                notificationPackages.Add(new PushNotificationPackage(content, subject, student.DeviceEndpoint, student.Id, student.DeviceToken, student.DeviceType));
            }

            // Email
            if (student.EmailConfirmed)
            {
                subject = StringAdapter.Format(ResourceValue(ResourceConstants.EduStudentPocketMoneyMailSubject, student.DefaultLanguage), parent.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName));

                _configurationService.ReadConfiguration("PaparaWebUrl", out string url, false);

                url += "personal/#!/edu-student";

                var dynamicContent = StringAdapter.Format(ResourceValue(ResourceConstants.EduStudentPocketMoneyMailDynamicContent, student.DefaultLanguage), parent.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName), period.ToLower(), amount, currencyCode);

                var package = EmailTemplates.GetEduMailPackage(subject, student.FullName, dynamicContent, url, new List<string> { student.Email }, student.DefaultLanguage);

                notificationPackages.Add(package);
            }

            // SMS
            if (!student.CanPushMobileNotification && !student.EmailConfirmed && !string.IsNullOrWhiteSpace(student.PhoneNumber))
            {
                content = StringAdapter.Format(ResourceValue(ResourceConstants.EduStudentPocketMoneySmsContent, student.DefaultLanguage), parent.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName), period, amount, currencyCode);

                notificationPackages.Add(new SmsNotificationPackage(content, student.PhoneNumber));
            }

            return notificationPackages;
        }

        /// <summary>
        /// Veli olma bildirmleri
        /// </summary>
        public static IList<NotificationPackage> EduAuthorizeParent(User parent, User student)
        {
            var notificationPackages = new List<NotificationPackage>();

            string content, subject;

            // Notification
            if (parent.CanPushMobileNotification)
            {
                subject = ResourceValue(ResourceConstants.EduAuthenticateParentNotificationSubject, parent.DefaultLanguage);

                content = StringAdapter.Format(ResourceValue(ResourceConstants.EduAuthenticateParentNotificationContent, parent.DefaultLanguage), student.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName));

                notificationPackages.Add(new PushNotificationPackage(content, subject, parent.DeviceEndpoint, parent.Id, parent.DeviceToken, parent.DeviceType));
            }

            // Email
            if (parent.EmailConfirmed)
            {
                subject = ResourceValue(ResourceConstants.EduAuthenticateParentMailSubject, parent.DefaultLanguage);

                _configurationService.ReadConfiguration("PaparaWebUrl", out string url, false);

                url += "personal/#!/pocket-money";

                var dynamicContent = StringAdapter.Format(ResourceValue(ResourceConstants.EduAuthorizeParentMailDynamicContent, parent.DefaultLanguage), student.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName));

                var package = EmailTemplates.GetEduMailPackage(subject, parent.FullName, dynamicContent, url, new List<string> { parent.Email }, parent.DefaultLanguage);

                notificationPackages.Add(package);
            }

            // SMS
            if (!parent.CanPushMobileNotification && !parent.EmailConfirmed && !string.IsNullOrWhiteSpace(parent.PhoneNumber))
            {
                content = StringAdapter.Format(ResourceValue(ResourceConstants.EduAuthenticateParentSmsContent, parent.DefaultLanguage), student.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName));

                notificationPackages.Add(new SmsNotificationPackage(content, parent.PhoneNumber));
            }

            return notificationPackages;
        }

        /// <summary>
        /// Öğrenci olma bildirmleri
        /// </summary>
        public static IList<NotificationPackage> EduAddStudent(User parent, User student)
        {
            var notificationPackages = new List<NotificationPackage>();

            string content, subject;

            if (student.CanPushMobileNotification)
            {
                subject = ResourceValue(ResourceConstants.EduAddStudentNotificationSubject, student.DefaultLanguage);

                content = StringAdapter.Format(ResourceValue(ResourceConstants.EduAddStudentNotificationContent, student.DefaultLanguage), parent.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName));

                notificationPackages.Add(new PushNotificationPackage(content, subject, student.DeviceEndpoint, student.Id, student.DeviceToken, student.DeviceType));
            }

            if (student.EmailConfirmed)
            {
                subject = ResourceValue(ResourceConstants.EduAddStudentMailSubject, student.DefaultLanguage);

                _configurationService.ReadConfiguration("PaparaWebUrl", out string url, false);

                url += "personal/#!/edu-student";

                var dynamicContent = StringAdapter.Format(ResourceValue(ResourceConstants.EduAddStudentMailDynamicContent, student.DefaultLanguage), parent.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName));

                var package = EmailTemplates.GetEduMailPackage(subject, student.FullName, dynamicContent, url, new List<string> { student.Email }, student.DefaultLanguage);

                notificationPackages.Add(package);
            }

            if (!student.CanPushMobileNotification && !student.EmailConfirmed && !string.IsNullOrWhiteSpace(student.PhoneNumber))
            {
                content = StringAdapter.Format(ResourceValue(ResourceConstants.EduAddStudentSmsContent, student.DefaultLanguage), parent.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName));

                notificationPackages.Add(new SmsNotificationPackage(content, student.PhoneNumber));
            }

            return notificationPackages;
        }

        /// <summary>
        /// Öğrenci edu kart internete açma/kapama bildirmleri
        /// </summary>
        public static IList<NotificationPackage> EduOnlineSpendingSettingChange(User parent, User student, bool internetEnabled)
        {
            var notificationPackages = new List<NotificationPackage>();

            string content, subject;

            if (student.CanPushMobileNotification)
            {
                subject = internetEnabled
                    ? ResourceValue(ResourceConstants.EduOnlineSpendingAllowedNotificationTitle, student.DefaultLanguage)
                    : ResourceValue(ResourceConstants.EduOnlineSpendingNotAllowedNotificationTitle, student.DefaultLanguage);

                content = internetEnabled
                    ? StringAdapter.Format(ResourceValue(ResourceConstants.EduOnlineSpendingAllowedNotificationContent, student.DefaultLanguage), parent.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName))
                    : StringAdapter.Format(ResourceValue(ResourceConstants.EduOnlineSpendingNotAllowedNotificationContent, student.DefaultLanguage), parent.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName));

                notificationPackages.Add(new PushNotificationPackage(content, subject, student.DeviceEndpoint, student.Id, student.DeviceToken, student.DeviceType));
            }

            return notificationPackages;
        }

        /// <summary>
        /// Öğrenci edu kart kantin dışı kullanıma açma/kapama bildirmleri
        /// </summary>
        public static IList<NotificationPackage> EduOnlyTerminalSettingChange(User parent, User student, bool onlyTerminalEnabled)
        {
            var notificationPackages = new List<NotificationPackage>();

            string content, subject;

            if (student.CanPushMobileNotification)
            {
                subject = onlyTerminalEnabled
                    ? ResourceValue(ResourceConstants.EduSpendingNotAllowedNotificationTitle, student.DefaultLanguage)
                    : ResourceValue(ResourceConstants.EduSpendingAllowedNotificationTitle, student.DefaultLanguage);

                content = onlyTerminalEnabled
                    ? StringAdapter.Format(ResourceValue(ResourceConstants.EduSpendingNotAllowedNotificationContent, student.DefaultLanguage), parent.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName))
                    : StringAdapter.Format(ResourceValue(ResourceConstants.EduSpendingAllowedNotificationContent, student.DefaultLanguage), parent.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName));

                notificationPackages.Add(new PushNotificationPackage(content, subject, student.DeviceEndpoint, student.Id, student.DeviceToken, student.DeviceType));
            }

            return notificationPackages;
        }

        /// <summary>
        /// Harçlık gönderme bildirmleri
        /// </summary>
        public static IList<NotificationPackage> PocketMoneyReceivedNotification(User parent, User student, decimal amount)
        {
            var notificationPackages = new List<NotificationPackage>();

            var subject = ResourceValue(ResourceConstants.IncomingPaymentNotificationTitle, student.DefaultLanguage);

            if (student.CanPushMobileNotification)
            {
                var content = StringAdapter.Format(ResourceValue(ResourceConstants.PocketMoneyReceivedEmailTitleAndNotificationBody, student.DefaultLanguage), parent.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName), amount.ToMoneyString(true));

                notificationPackages.Add(new PushNotificationPackage(content, subject, student.DeviceEndpoint, student.Id, student.DeviceToken, student.DeviceType));
            }

            var userConfirmedEmail = student.EmailConfirmed;

            subject = StringAdapter.Format(ResourceValue(ResourceConstants.PocketMoneyReceivedEmailTitleAndNotificationBody, student.DefaultLanguage), parent.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName), amount.ToMoneyString(true));

            if (userConfirmedEmail)
            {
                var package = EmailTemplates.GetPocketMoneyReceivedEmailPackage(parent, student, amount, CurrencyService.DefaultCurrencyInfo, subject, new List<string> { student.Email });

                notificationPackages.Add(package);
            }

            var studentHasPhoneNumber = !string.IsNullOrEmpty(student.PhoneNumber);

            if (!userConfirmedEmail && !student.CanPushMobileNotification && studentHasPhoneNumber)
            {
                notificationPackages.Add(new SmsNotificationPackage(subject, student.PhoneNumber));
            }

            return notificationPackages;
        }

        /// <summary>
        /// Harçlık gönderme hata bildirmleri
        /// </summary>
        public static IList<NotificationPackage> FailedDefinedPocketMoney(User parent, ServiceResult serviceResult)
        {
            var notificationPackages = new List<NotificationPackage>();

            var smsNotification = true;

            var notificationContent = serviceResult.Error.Message;
            var smsContent = serviceResult.Error.Message;
            var emailContent = serviceResult.Error.Message;

            SetFailedNotificationContentsAccordingToServiceErrorCodes(serviceResult, ref notificationContent, ref smsContent, ref emailContent, parent);

            if (parent.CanPushMobileNotification)
            {
                notificationPackages.Add(new PushNotificationPackage(notificationContent, ResourceValue(ResourceConstants.PocketMoneyInstructionErrorNotificationTitle, parent.DefaultLanguage), parent.DeviceEndpoint, parent.Id, parent.DeviceToken, parent.DeviceType));

                smsNotification = false;
            }

            if (parent.EmailConfirmed)
            {
                notificationPackages.Add(EmailTemplates.GetPocketMoneyInstructionFailedMailPackage(parent, emailContent, ResourceValue(ResourceConstants.PocketMoneyInstructionErrorNotificationTitle, parent.DefaultLanguage), new List<string> { parent.Email }));

                smsNotification = false;
            }

            if (smsNotification)
            {
                // Eğer kullanıcı eposta adresini onaylamadıysa ve uygulama üzerinden bildirim gönderemiyorsak, sms gönderiyoruz.
                notificationPackages.Add(new SmsNotificationPackage(smsContent, parent.PhoneNumber));
            }

            return notificationPackages;
        }

        /// <summary>
        /// Sets notification contents according to getting service error of the service result.
        /// </summary>
        private static void SetFailedNotificationContentsAccordingToServiceErrorCodes(ServiceResult serviceResult, ref string notificationContent, ref string smsContent, ref string emailContent, User user)
        {
            // Bakiye yetersizse bunu ayrı olarak bildirmek istediğimiz için detaylı mesaj gönderiyoruz.
            // Kritik nokta en fazla 160 karakter olması gerekiyor, bildirim ve sms için.
            // 105 bakiye yetersiz hatası
            if (serviceResult.Error.Code == 105)
            {
                notificationContent = ResourceValue(ResourceConstants.EduPocketMoneyInsufficientBalanceNotification, user?.DefaultLanguage);
                smsContent = ResourceValue(ResourceConstants.EduPocketMoneyInsufficientBalanceSms, user?.DefaultLanguage);
                emailContent = ResourceValue(ResourceConstants.EduPocketMoneyInsufficientBalanceMail, user?.DefaultLanguage);
            }

            // 999999 kredi kartı limit yetersizliği için tanımlanmış bir kod.
            if (serviceResult.Error.Code == 999999)
            {
                notificationContent = ResourceValue(ResourceConstants.EduPocketMoneyInsufficientBalanceForCreditCardNotification, user?.DefaultLanguage);
                emailContent = ResourceValue(ResourceConstants.EduPocketMoneyInsufficientBalanceForCreditCardMail, user?.DefaultLanguage);
                smsContent = ResourceValue(ResourceConstants.EduPocketMoneyInsufficientBalanceForCreditCardSms, user?.DefaultLanguage);
            }
        }

        /// <summary>
        /// Düzenli para gönderme hata bildirmleri (ServiceResult)
        /// </summary>
        public static IList<NotificationPackage> FailedDefinedRecurringMoneyTransfer(User user, ServiceResult serviceResult)
        {
            var notificationPackages = new List<NotificationPackage>();

            var smsNotification = true;

            var notificationContent = serviceResult.Error.Message;
            var smsContent = serviceResult.Error.Message;
            var emailContent = StringAdapter.Format(serviceResult.Error.Message, ResourceValue(ResourceConstants.RecurringMoneyTransferInstructionErrorMailBody, user.DefaultLanguage));

            if (user.CanPushMobileNotification)
            {
                notificationPackages.Add(new PushNotificationPackage(notificationContent, ResourceValue(ResourceConstants.RecurringMoneyTransferInstructionErrorNotificationTitle, user.DefaultLanguage), user.DeviceEndpoint, user.Id, user.DeviceToken, user.DeviceType));

                smsNotification = false;
            }

            if (user.EmailConfirmed)
            {
                notificationPackages.Add(EmailTemplates.GetRecurringMoneyTransferInstructionFailedMailPackage(user, emailContent, ResourceValue(ResourceConstants.RecurringMoneyTransferInstructionErrorNotificationTitle, user.DefaultLanguage), new List<string> { user.Email }));

                smsNotification = false;
            }

            if (smsNotification)
            {
                // Eğer kullanıcı eposta adresini onaylamadıysa ve uygulama üzerinden bildirim gönderemiyorsak, sms gönderiyoruz.
                notificationPackages.Add(new SmsNotificationPackage(smsContent, user.PhoneNumber));
            }

            return notificationPackages;
        }

        /// <summary>
        /// Para çekme isteği tamamlanma bildirmleri
        /// </summary>
        public static IList<NotificationPackage> ConfirmWithdrawalRequestNotification(User user, decimal amount, CurrencyInfo currencyInfo, string bankShortName)
        {
            var notificationPackages = new List<NotificationPackage>();

            amount = Math.Abs(amount);

            if (user.CanPushMobileNotification)
            {
                var content = StringAdapter.Format(ResourceValue(ResourceConstants.ConfirmWithdrawalNotificationMessage, user.DefaultLanguage), bankShortName, amount.ToMoneyString(rounded: true), currencyInfo.PreferredDisplayCode);

                var subject = ResourceValue(ResourceConstants.ConfirmWithdrawalNotificationTitle, user.DefaultLanguage);

                notificationPackages.Add(new PushNotificationPackage(content, subject, user.DeviceEndpoint, user.Id, user.DeviceToken, user.DeviceType));
            }

            if (user.EmailConfirmed)
            {
                _configurationService.ReadConfiguration("PaparaWebUrl", out string url, false);

                var subject = StringAdapter.Format(ResourceValue(ResourceConstants.ConfirmWithdrawalNotificationTitleWithAmount, user.DefaultLanguage), amount.ToMoneyString(rounded: true), currencyInfo.PreferredDisplayCode);

                var package = EmailTemplates.GetConfirmWithdrawalRequestMailPackage(user, Math.Abs(amount), currencyInfo.PreferredDisplayCode, bankShortName, url, subject, new List<string> { user.Email });

                notificationPackages.Add(package);
            }

            return notificationPackages;
        }

        /// <summary>
        /// Para çekme isteği iptal etme bildirmleri
        /// </summary>
        public static IList<NotificationPackage> RefundWithdrawalRequestNotification(User user, decimal amount, CurrencyInfo currencyInfo, string bankShortName)
        {
            var notificationPackages = new List<NotificationPackage>();

            amount = Math.Abs(amount);

            if (user.CanPushMobileNotification)
            {
                var content = StringAdapter.Format(ResourceValue(ResourceConstants.RejectedWithdrawalNotificationMessage, user.DefaultLanguage), bankShortName, amount.ToMoneyString(rounded: true), currencyInfo.PreferredDisplayCode);

                var subject = ResourceValue(ResourceConstants.RejectedWithdrawalNotificationTitle, user.DefaultLanguage);

                notificationPackages.Add(new PushNotificationPackage(content, subject, user.DeviceEndpoint, user.Id, user.DeviceToken, user.DeviceType));
            }

            if (user.EmailConfirmed)
            {
                _configurationService.ReadConfiguration("PaparaWebUrl", out string url, false);

                var subject = StringAdapter.Format(ResourceValue(ResourceConstants.RejectedWithdrawalMailTitle, user.DefaultLanguage), amount.ToMoneyString(rounded: true),
                    currencyInfo.PreferredDisplayCode);

                var package = EmailTemplates.GetRefundWithdrawalRequestMailPackage(user, Math.Abs(amount), currencyInfo.PreferredDisplayCode, bankShortName, url, subject, new List<string> { user.Email });

                notificationPackages.Add(package);
            }

            return notificationPackages;
        }

        /// <summary>
        /// Para çekme isteği iptal etme bildirmleri
        /// </summary>
        public static IList<NotificationPackage> RefundIbanMoneyTransferNotification(User user, decimal amount, CurrencyInfo currencyInfo, string bankShortName, string iban)
        {
            var notificationPackages = new List<NotificationPackage>();

            amount = Math.Abs(amount);

            if (user.CanPushMobileNotification)
            {
                var content = StringAdapter.Format(ResourceValue(ResourceConstants.RejectedIbanMoneyTransferNotificationMessage, user.DefaultLanguage), bankShortName, amount.ToMoneyString(rounded: true), currencyInfo.PreferredDisplayCode);

                var subject = ResourceValue(ResourceConstants.RejectedIbanMoneyTransferNotificationTitle, user.DefaultLanguage);

                notificationPackages.Add(new PushNotificationPackage(content, subject, user.DeviceEndpoint, user.Id, user.DeviceToken, user.DeviceType));
            }

            if (user.EmailConfirmed)
            {
                _configurationService.ReadConfiguration("PaparaWebUrl", out string url, false);

                var subject = StringAdapter.Format(ResourceValue(ResourceConstants.RejectedIbanMoneyTransferMailTitle, user.DefaultLanguage), amount.ToMoneyString(rounded: true),
                    currencyInfo.PreferredDisplayCode);

                var package = EmailTemplates.GetRefundIbanMoneyTransferMailPackage(user, Math.Abs(amount), currencyInfo.PreferredDisplayCode, bankShortName, iban, url, subject, new List<string> { user.Email });

                notificationPackages.Add(package);
            }

            return notificationPackages;
        }

        /// <summary>
        /// Ibana para gönderme isteği tamamlanma bildirmleri
        /// </summary>
        public static IList<NotificationPackage> IbanMoneyTransferRequestNotification(User user, decimal amount, CurrencyInfo currencyInfo, string iban, string bankName, bool ibanActive, string ibanDisplayName, bool isRecurring = false)
        {
            var notificationPackages = new List<NotificationPackage>();

            amount = Math.Abs(amount);

            if (isRecurring)
            {
                if (user.CanPushMobileNotification)
                {
                    var description = bankName;

                    if (ibanActive)
                    {
                        description = ibanDisplayName;
                    }

                    var content = StringAdapter.Format(ResourceValue(ResourceConstants.ConfirmRecurringIbanMoneyTransferNotificationMessage, user.DefaultLanguage), description, amount.ToMoneyString(rounded: true), currencyInfo.PreferredDisplayCode);

                    var subject = ResourceValue(ResourceConstants.ConfirmRecurringIbanMoneyTransferNotificationTitle, user.DefaultLanguage);

                    notificationPackages.Add(new PushNotificationPackage(content, subject, user.DeviceEndpoint, user.Id, user.DeviceToken, user.DeviceType));
                }

                if (user.EmailConfirmed)
                {
                    _configurationService.ReadConfiguration("PaparaWebUrl", out string url, false);

                    var titleResourceValue = ResourceValue(ResourceConstants.ConfirmRecurringIbanMoneyTransferEmailTitle, user.DefaultLanguage);

                    var title = StringAdapter.Format(titleResourceValue, amount.ToMoneyString(rounded: true), currencyInfo.PreferredDisplayCode);

                    var package = EmailTemplates.GetRecurringConfirmIbanMoneyTransferMailPackage(user, Math.Abs(amount), currencyInfo.PreferredDisplayCode, iban, bankName, url, title, new List<string> { user.Email });

                    notificationPackages.Add(package);
                }

                return notificationPackages;
            }



            if (user.CanPushMobileNotification)
            {
                var description = bankName;

                if (ibanActive)
                {
                    description = ibanDisplayName;
                }

                var content = StringAdapter.Format(ResourceValue(ResourceConstants.ConfirmIbanMoneyTransferNotificationMessage, user.DefaultLanguage), description, amount.ToMoneyString(rounded: true), currencyInfo.PreferredDisplayCode);

                var subject = ResourceValue(ResourceConstants.ConfirmIbanMoneyTransferNotificationTitle, user.DefaultLanguage);

                notificationPackages.Add(new PushNotificationPackage(content, subject, user.DeviceEndpoint, user.Id, user.DeviceToken, user.DeviceType));
            }

            if (user.EmailConfirmed)
            {
                _configurationService.ReadConfiguration("PaparaWebUrl", out string url, false);

                var titleResourceValue = ResourceValue(ResourceConstants.ConfirmIbanMoneyTransferEmailTitle, user.DefaultLanguage);

                var title = StringAdapter.Format(titleResourceValue, amount.ToMoneyString(rounded: true), currencyInfo.PreferredDisplayCode);

                var package = EmailTemplates.GetConfirmIbanMoneyTransferMailPackage(user, Math.Abs(amount), currencyInfo.PreferredDisplayCode, iban, bankName, url, title, new List<string> { user.Email });

                notificationPackages.Add(package);
            }

            return notificationPackages;
        }

        /// <summary>
        /// Kısıtlama Listesi Kayıtlı Kullanıcı Bildirimi
        /// </summary>
        public static IList<NotificationPackage> SanctionScanResultNotification(List<string> to, byte[] attachment, string attachmentFileName)
        {
            var notificationPackages = new List<NotificationPackage>();

            var package = EmailTemplates.GetSanctionListDeniedUserMailPackage(ResourcesService.GetInDomainsDefaultLanguage(ResourceConstants.SanctionScanResultEmailSubject), to, attachment, attachmentFileName);

            notificationPackages.Add(package);

            return notificationPackages;
        }

        /// <summary>
        /// Para yatırma iade Papara numarası yanlış bildirmleri
        /// </summary>
        public static IList<NotificationPackage> DepositRefundInvalidPaparaNumberNotification(User user, decimal amount, CurrencyInfo currencyInfo)
        {
            var packages = new List<NotificationPackage>();

            if (user.CanPushMobileNotification)
            {
                var content = StringAdapter.Format(ResourceValue(ResourceConstants.DepositRefundInvalidPaparaNumberNotificationMessage, user.DefaultLanguage), amount.ToMoneyString(rounded: true), currencyInfo.PreferredDisplayCode);

                packages.Add(new PushNotificationPackage(content, ResourceValue(ResourceConstants.DepositRefundInvalidPaparaNumberNotificationTitle, user.DefaultLanguage), user.DeviceEndpoint, user.Id, user.DeviceToken, user.DeviceType));
            }

            if (user.EmailConfirmed)
            {
                var package = EmailTemplates.GetDepositRefundInvalidPaparaNumberInfoMailPackage(user, amount, currencyInfo.PreferredDisplayCode, ResourceValue(ResourceConstants.DepositRefundInvalidPaparaNumberEmailTitle, user.DefaultLanguage), new List<string> { user.Email });

                packages.Add(package);
            }

            if (!user.CanPushMobileNotification)
            {
                var content = StringAdapter.Format(ResourceValue(ResourceConstants.DepositRefundInvalidPaparaNumberSmsMessage, user.DefaultLanguage), amount.ToMoneyString(rounded: true), currencyInfo.PreferredDisplayCode);

                packages.Add(new SmsNotificationPackage(content, user.PhoneNumber));
            }

            return packages;
        }

        /// <summary>
        /// Para yatırma iade Limit aşımı bildirmleri
        /// </summary>
        public static IList<NotificationPackage> DepositRefundLimitExceededNotification(User user, decimal amount, CurrencyInfo currencyInfo, decimal limit)
        {
            var packages = new List<NotificationPackage>();

            if (user.CanPushMobileNotification)
            {
                var content = StringAdapter.Format(ResourceValue(ResourceConstants.DepositRefundLimitExceededNotificationMessage, user.DefaultLanguage), amount.ToMoneyString(rounded: true), currencyInfo.PreferredDisplayCode);

                packages.Add(new PushNotificationPackage(content, ResourceValue(ResourceConstants.DepositRefundLimitExceededNotificationTitle, user.DefaultLanguage), user.DeviceEndpoint, user.Id, user.DeviceToken, user.DeviceType));
            }

            if (user.EmailConfirmed)
            {
                var package = EmailTemplates.GetDepositRefundLimitExceededInfoMailPackage(user, amount, currencyInfo.PreferredDisplayCode, limit, ResourceValue(ResourceConstants.DepositRefundLimitExceededEmailTitle, user.DefaultLanguage), new List<string> { user.Email });

                packages.Add(package);
            }

            if (!user.CanPushMobileNotification)
            {
                var content = StringAdapter.Format(ResourceValue(ResourceConstants.DepositRefundLimitExceededSmsMessage, user.DefaultLanguage), amount.ToMoneyString(rounded: true), currencyInfo.PreferredDisplayCode);

                packages.Add(new SmsNotificationPackage(content, user.PhoneNumber));
            }

            return packages;
        }

        /// <summary>
        /// Para yatırma tutar aşımı bildirmleri
        /// </summary>
        public static IList<NotificationPackage> DepositRefundBankRejectNotification(User user, decimal amount, CurrencyInfo currencyInfo)
        {
            var packages = new List<NotificationPackage>();

            if (user.CanPushMobileNotification)
            {
                var content = StringAdapter.Format(ResourceValue(ResourceConstants.DepositRefundBankRejectNotificationMessage, user.DefaultLanguage), amount.ToMoneyString(rounded: true), currencyInfo.PreferredDisplayCode);

                packages.Add(new PushNotificationPackage(content, ResourceValue(ResourceConstants.DepositRefundBankRejectNotificationTitle, user.DefaultLanguage), user.DeviceEndpoint, user.Id, user.DeviceToken, user.DeviceType));
            }

            if (user.EmailConfirmed)
            {
                var package = EmailTemplates.GetDepositRefundBankRejectedInfoMailPackage(user, amount, currencyInfo.PreferredDisplayCode, ResourceValue(ResourceConstants.DepositRefundBankRejectEmailTitle, user.DefaultLanguage), new List<string> { user.Email });

                packages.Add(package);
            }

            if (!user.CanPushMobileNotification)
            {
                var content = StringAdapter.Format(ResourceValue(ResourceConstants.DepositRefundBankRejectSmsMessage, user.DefaultLanguage), amount.ToMoneyString(rounded: true), currencyInfo.PreferredDisplayCode);

                packages.Add(new SmsNotificationPackage(content, user.PhoneNumber));
            }

            return packages;
        }

        #endregion

        #region Papara Card Notifications

        //Todo: This will be stay here without static. when all notification services implemented.
        /// <summary>
        /// Base method for declinedPaparaCardTransactionInfoNotifications
        /// </summary>
        public static IList<NotificationPackage> TransactionDeclinedInfoNotification(User user,
            string notificationTitle,
            string notificationContent,
            string emailContent,
            Dictionary<string, string> additionalMailParameters,
            dynamic customData = null)
        {
            var notificationPackages = new List<NotificationPackage>();

            //Kullanıcı adı ve mail içeriği hariç farklı parametreler notification content içine basılmalı.
            if (additionalMailParameters?.Count > 0)
            {
                notificationContent = Regex.Replace(additionalMailParameters.Aggregate(notificationContent, (current, pair) => current.Replace($"<%={pair.Key}%>", pair.Value)), @"\s{2,}", " ");

                emailContent = Regex.Replace(additionalMailParameters.Aggregate(emailContent, (current, pair) => current.Replace($"<%={pair.Key}%>", pair.Value)), @"\s{2,}", " ");

                notificationTitle = additionalMailParameters.Aggregate(notificationTitle, (current, pair) => current.Replace($"<%={pair.Key}%>", pair.Value));
            }

            if (user.CanPushMobileNotification)
            {
                notificationPackages.Add(new PushNotificationPackage(notificationContent, notificationTitle, user.DeviceEndpoint, user.Id, user.DeviceToken, user.DeviceType,
                    customData));
            }

            if (user.EmailConfirmed)
            {
                _configurationService.ReadConfiguration("PaparaWebUrl", out string url, false);

                notificationPackages.Add(EmailTemplates.GetPosTransactionNotSucceededInfoMailPackage(UserNotificationDto.Create(user), notificationTitle,
                    emailContent, url, new List<string> { user.Email }));
            }
            else
            {
                notificationPackages.Add(new SmsNotificationPackage(notificationContent, user.PhoneNumber));
            }

            return notificationPackages;
        }

        #endregion

        /// <summary>
        /// Öğrencinin bakiyesi X altına düştüğünde veli tarafına kritik bakiye bildirimi gönderilir
        /// </summary>
        public static IList<NotificationPackage> StudentBalanceCriticalNotification(User parent, User student, decimal studentBalance, CurrencyInfo currencyInfo)
        {
            var notificationPackages = new List<NotificationPackage>();

            string content, subject;

            // Notification
            if (parent.CanPushMobileNotification)
            {
                subject = StringAdapter.Format(ResourceValue(ResourceConstants.EduEarlyBirdAllowanceNoticeSubject, parent.DefaultLanguage), studentBalance.ToMoneyString(rounded: true), currencyInfo.PreferredDisplayCode);

                content = StringAdapter.Format(ResourceValue(ResourceConstants.EduEarlyBirdAllowanceNoticeContent, parent.DefaultLanguage), studentBalance.ToMoneyString(rounded: true), currencyInfo.PreferredDisplayCode);

                notificationPackages.Add(new PushNotificationPackage(content, subject, parent.DeviceEndpoint, parent.Id, parent.DeviceToken, parent.DeviceType));
            }

            // Email
            if (parent.EmailConfirmed)
            {
                subject = StringAdapter.Format(ResourceValue(ResourceConstants.EduEarlyBirdAllowanceMailSubject, parent.DefaultLanguage), studentBalance.ToMoneyString(rounded: true), currencyInfo.PreferredDisplayCode);

                var package = EmailTemplates.GetStudentBalanceCriticalNotification(student, parent, currencyInfo, studentBalance, subject, new List<string> { parent.Email });

                notificationPackages.Add(package);
            }

            return notificationPackages;
        }

        ///<summary>
        /// Notification for accepting commercial account request
        /// </summary>
        public static IList<NotificationPackage> CommercialAccountAccept(CommercialAccountApplication application)
        {
            var notificationPackages = new List<NotificationPackage>();

            var subject = ResourceValue(ResourceConstants.CommercialApplicationApprovedNotificationTitle, application.User?.DefaultLanguage);

            _configurationService.ReadConfiguration("PaparaWebUrl", out string url, false);

            // Email
            if (application.User.EmailConfirmed)
            {
                var package = EmailTemplates.CommercialAccountAcceptMailPackage(application.User, subject,
                    new List<string> { application.User.Email }, url);

                notificationPackages.Add(package);
            }

            // Notification
            if (application.User.CanPushMobileNotification)
            {
                string content = ResourceValue(ResourceConstants.CommercialApplicationApprovedNotificationBody, application.User?.DefaultLanguage);

                dynamic customData = new
                {
                    commercialApplication = new
                    {
                        id = application.Id,
                        status = application.Status,
                        jobType = application.User.JobType
                    }
                };

                notificationPackages.Add(new PushNotificationPackage(content, subject, application.User.DeviceEndpoint, application.User.Id, application.User.DeviceToken, application.User.DeviceType, customData));
            }

            return notificationPackages;
        }

        /// <summary>
        /// Notification for rejecting commercial account request
        /// </summary>
        public static IList<NotificationPackage> CommercialAccountReject(User user, string reasonForRejection)
        {
            var notificationPackages = new List<NotificationPackage>();

            var subject = ResourceValue(ResourceConstants.CommercialApplicationRejectedNotificationTitle, user.DefaultLanguage);

            _configurationService.ReadConfiguration("PaparaWebUrl", out string url, false);

            // Email
            if (user.EmailConfirmed)
            {
                var package = EmailTemplates.CommercialAccountRejectMailPackage(user, reasonForRejection, subject,
                    new List<string> { user.Email }, url);

                notificationPackages.Add(package);
            }

            // Notification
            if (user.CanPushMobileNotification)
            {
                string content = ResourceValue(ResourceConstants.CommercialApplicationRejectedNotificationBody, user.DefaultLanguage);

                dynamic customData = new
                {
                    commercialApplication = new
                    {
                        status = CommercialApplicationStatus.Rejected,
                        reasonForRejection
                    }
                };

                notificationPackages.Add(new PushNotificationPackage(content, subject, user.DeviceEndpoint, user.Id, user.DeviceToken, user.DeviceType));
            }

            return notificationPackages;
        }

        /// <summary>
        /// Notification for reopening commercial account request for some needs
        /// </summary>
        public static IList<NotificationPackage> CommercialAccountReopened(CommercialAccountApplication application, string reasonForReopening)
        {
            var notificationPackages = new List<NotificationPackage>();

            var subject = ResourceValue(ResourceConstants.CommercialApplicationReOpenedNotificationTitle, application.User?.DefaultLanguage);

            _configurationService.ReadConfiguration("PaparaWebUrl", out string url, false);

            // Email
            if (application.User.EmailConfirmed)
            {
                var package = EmailTemplates.CommercialAccountReopenMailPackage(application.User, reasonForReopening, subject,
                    new List<string> { application.User.Email }, url);

                notificationPackages.Add(package);
            }

            // Notification
            if (application.User.CanPushMobileNotification)
            {
                string content = ResourceValue(ResourceConstants.CommercialApplicationReOpenedNotificationBody, application.User?.DefaultLanguage);

                dynamic customData = new
                {
                    commercialApplication = new
                    {
                        id = application.Id,
                        status = application.Status,
                        jobType = application.User.JobType
                    }
                };

                notificationPackages.Add(new PushNotificationPackage(content, subject, application.User.DeviceEndpoint, application.User.Id, application.User.DeviceToken, application.User.DeviceType, customData));
            }

            return notificationPackages;
        }

        /// <summary>
        /// Papara Card saving with spend rounding notification succeeded
        /// </summary>
        public static IList<NotificationPackage> SpendRoundingSavingMoneySucceededNotification(User user, string pan, string cardTypeName, decimal amount, decimal savingAmount, CurrencyInfo currency, string acceptorName)
        {
            var title = ResourceValue(ResourceConstants.SpendRoundingSavingNotificationTitle, user.DefaultLanguage);

            var contentResource = ResourceValue(ResourceConstants.SpendRoundingSavingNotificationContent, user.DefaultLanguage);

            return SpendRoundingSavingMoneyNotification(contentResource, title, user, pan, cardTypeName, amount, savingAmount, currency, acceptorName);
        }

        /// <summary>
        /// Papara Card saving with spend rounding notification fail Insufficient Balance
        /// </summary>
        public static IList<NotificationPackage> SpendRoundingSavingMoneyInsufficientBalanceNotification(User user,
            string pan,
            string cardTypeName,
            decimal amount,
            decimal savingAmount,
            CurrencyInfo currency,
            string acceptorName)
        {
            var title = ResourceValue(ResourceConstants.SpendRoundingSavingInsufficientBalanceNotificationTitle, user.DefaultLanguage);

            var contentResource = ResourceValue(ResourceConstants.SpendRoundingSavingInsufficientBalanceNotificationContent, user.DefaultLanguage);

            return SpendRoundingSavingMoneyNotification(contentResource, title, user, pan, cardTypeName, amount, savingAmount, currency, acceptorName);
        }

        /// <summary>
        /// Papara Card saving with spend rounding notification
        /// </summary>
        private static IList<NotificationPackage> SpendRoundingSavingMoneyNotification(string contentResource,
            string title,
            User user,
            string pan,
            string cardTypeName,
            decimal amount,
            decimal savingAmount,
            CurrencyInfo currency,
            string acceptorName)
        {
            var notificationPackages = new List<NotificationPackage>();

            if (user.CanPushMobileNotification)
            {
                var content = StringAdapter.Format(contentResource, pan, cardTypeName, Math.Abs(amount), currency.PreferredDisplayCode, acceptorName.TruncateCardAceptorName(24), Math.Abs(savingAmount), currency.PreferredDisplayCode);

                notificationPackages.Add(new PushNotificationPackage(content, title, user.DeviceEndpoint, user.Id, user.DeviceToken, user.DeviceType));
            }

            return notificationPackages;
        }

        /// <summary>
        /// Returns email package for create saving account
        /// </summary>
        public static EmailNotificationPackage CreateSavingAccountNotification(User user, CurrencyInfo currencyInfo)
        {
            _configurationService.ReadConfiguration("PaparaWebUrl", out string url, true);

            var subject = StringAdapter.Format(ResourceValue(ResourceConstants.CreateSavingAccountMailSubject, user.DefaultLanguage), currencyInfo.Name);

            if (user.EmailConfirmed)
            {
                return EmailTemplates.GetCreateSavingAccountMailPackage(user, currencyInfo, url, subject,
                    new List<string> { user.Email });
            }

            return null;
        }

        /// <summary>
        /// User LockedOut after max failed access attempts mail notification
        /// </summary>
        public static EmailNotificationPackage UserLockedOutWithMaxFailedAccessAttemptsNotification(User user)
        {
            if (user.EmailConfirmed)
            {
                string content, subject;

                subject = StringAdapter.Format(ResourceValue(ResourceConstants.UserLockedOutWithMaxFailedAccessAttemptsSubject, user?.DefaultLanguage));

                content = ResourceValue(ResourceConstants.UserLockedOutWithMaxFailedAccessAttemptsContent, user?.DefaultLanguage);

                return EmailTemplates.GetUserLockedOutWithMaxFailedAccessAttemptsMailPackage(user.DefaultLanguage, subject, user.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName), content, new List<string> { user.Email });
            }

            return null;
        }

        /// <summary>
        /// Compliance Team automated user lock mail notification
        /// </summary>
        public static EmailNotificationPackage ComplianceLockNotification(User user)
        {
            if (user.EmailConfirmed)
            {
                string content, subject;

                subject = StringAdapter.Format(ResourceValue(ResourceConstants.UserComplianceLockSubject, user?.DefaultLanguage));

                content = ResourceValue(ResourceConstants.UserComplianceLockContent, user?.DefaultLanguage);

                return EmailTemplates.GetComplianceLockMailPackage(user.DefaultLanguage, subject, user.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName), content, new List<string> { user.Email });
            }

            return null;
        }

        /// <summary>
        /// Send ledger history as attachment to user
        /// </summary>
        public static EmailNotificationPackage GetComplianceLockReportNotification(ComplianceLockReportNotificationModel model)
        {
            var isVerifiedUsersIncludedString = model.IsVerifiedUsersIncluded ? "Evet" : "Hayır";
            var hasBankDepositString = model.HasBankDeposit ? "Evet" : "Hayır";

            return EmailTemplates.GetComplianceLockReportMailPackage(
                StringAdapter.Format(ResourceValue(ResourceConstants.ComplianceLockReportSubject)),
                StringAdapter.Format(ResourceValue(ResourceConstants.ComplianceLockReportMessage)),
                StringAdapter.Format(
                    ResourceValue(ResourceConstants.ComplianceLockReportDynamicContent),
                        model.CountOfLockedUsers,
                        model.CountOfAlreadyLockedUsersFromOperation,
                        model.CountOfNotFoundUsers,
                        model.CountOfNotLockedUsers,
                        isVerifiedUsersIncludedString,
                        model.CountOfDidNotMatchIsVerifiedUsersIncluded,
                        model.MaxUserBalanceMustBeLessThan.ToMoneyString(true, 2),
                        model.CountOfAvailableBalanceIsGreaterThanGivenAmount,
                        hasBankDepositString,
                        model.CountOfHasBankDepositNotIncluded,
                        model.CountOfAlreadyLockedUsersFromComplianceTeam),
                new List<string> { model.ComplianceMailAddress },
                model.AttachmentFile,
                model.AttachmentFileName);
        }

        /// <summary>
        /// Send report as attachment to user
        /// </summary>
        public static EmailNotificationPackage GetReportMailTemplate(User user, byte[] attachmentFile, string attachmentFileName)
        {
            return EmailTemplates.GetReportMailTemplate(
                ResourceValue(ResourceConstants.ReportMailTitle, user.DefaultLanguage),
                user.GetPrivacyInfoIfAvailable(UserPrivacyInfoType.FullName),
                ResourceValue(ResourceConstants.ReportMailContent, user.DefaultLanguage),
                new List<string> { user.Email },
                attachmentFile,
                attachmentFileName);
        }

        /// <summary>
        /// Gets merchant's payment report mail template
        /// </summary>
        public static EmailNotificationPackage GetMerchantPaymentReportMailTemplate(string name, string email, string language, byte[] attachmentFile, string attachmentFileName)
        {
            return EmailTemplates.GetReportMailTemplate(
                ResourceValue(ResourceConstants.ReportMailTitle, language),
                name,
                ResourceValue(ResourceConstants.ReportMailContent, language),
                new List<string> { email },
                attachmentFile,
                attachmentFileName);
        }

        /// <summary>
        /// Send ledger history as attachment to user
        /// </summary>
        public static EmailNotificationPackage GetLedgerHistoryMailTemplate(User user, string culture, string year, string month, byte[] attachmentFile, string attachmentFileName)
        {
            _configurationService.ReadConfiguration("PaparaWebUrl", out string url, false);

            url += "personal/#!/transaction-history";

            return EmailTemplates.GetLedgerHistoryMailTemplate(
                StringAdapter.Format(ResourceValue(ResourceConstants.LedgerHistoryMailTitle, culture), month, year),
                user.FullName,
                StringAdapter.Format(ResourceValue(ResourceConstants.LedgerHistoryMailContent, culture), month, year),
                url,
                new List<string> { user.Email },
                attachmentFile,
                attachmentFileName,
                culture);
        }

        /// <summary>
        /// Send ledgers as attachment to user
        /// </summary>
        public static EmailNotificationPackage GetLedgerReportMailTemplate(User user, byte[] attachmentFile, string attachmentFileName)
        {
            return EmailTemplates.GetReportMailTemplate(
                ResourceValue(ResourceConstants.ReportMailTitle, user.DefaultLanguage),
                user.FullName,
                ResourceValue(ResourceConstants.LedgerReportMailContent, user.DefaultLanguage),
                new List<string> { user.Email },
                attachmentFile,
                attachmentFileName);
        }

        /// <summary>
        /// Returns Monthly summary pdf as attachment
        /// </summary>
        public static EmailNotificationPackage GetMonthlySummaryMailTemplate(User user, string culture, int year, int month, byte[] attachmentFile, string attachmentFileName)
        {
            _configurationService.ReadConfiguration("PaparaWebUrl", out string url, false);
            string fullMonthName = new DateTime(year, month, 1).ToString("MMMM", CultureInfo.CreateSpecificCulture(culture));
            url += "personal/#!/monthlysummary";

            return EmailTemplates.GetLedgerHistoryMailTemplate(
                StringAdapter.Format(ResourceValue(ResourceConstants.MonthlySummaryMailTitle, culture), fullMonthName, year),
                user.FullName,
                StringAdapter.Format(ResourceValue(ResourceConstants.MonthlySummaryMailContent, culture), fullMonthName, year),
                url,
                new List<string> { user.Email },
                attachmentFile,
                attachmentFileName,
                culture);
        }

        /// <summary>
        /// Notifications for Credential on File transactions
        /// </summary>
        public static IList<NotificationPackage> CredentialOnFileNotaficationPackages(
            List<PaparaCardSubscription> subscriptions, User user, PaparaCard paparaCard, string endOfCardNumber,
            string paparaCardDisplayName)
        {
            var packages = new List<NotificationPackage>();

            if (!user.EmailConfirmed)
            {
                return packages;
            }

            string externalCardAcceptorList = "<ul>";

            foreach (var subscription in subscriptions)
            {
                externalCardAcceptorList += "<li>" + (subscription.ExternalCardAcceptor?.BrandName ?? subscription.BrandName) + "</li>";
            }

            externalCardAcceptorList += "</ul>";

            if (paparaCard.Status == PaparaCardStatus.Cancelled)
            {
                var emailTitle = StringAdapter.Format(ResourceValue(ResourceConstants.CredentialOnFileCanceledCardEmailTitle, user.DefaultLanguage), paparaCardDisplayName);

                var dynamicContent = StringAdapter.Format(ResourceValue(ResourceConstants.CredentialOnFileCanceledCardEmailContentIntro, user.DefaultLanguage),
                    endOfCardNumber, paparaCardDisplayName);

                var package = EmailTemplates.GetCredentialOnFileCanceledMailPackage(user, dynamicContent, externalCardAcceptorList, emailTitle, new List<string> { user.Email });

                packages.Add(package);
            }

            if (paparaCard.Status == PaparaCardStatus.Blocked)
            {
                var emailTitle = StringAdapter.Format(ResourceValue(ResourceConstants.CredentialOnFileBlockedCardEmailTitle, user.DefaultLanguage), paparaCardDisplayName);

                var dynamicContent = StringAdapter.Format(ResourceValue(ResourceConstants.CredentialOnFileBlockedCardEmailContentIntro, user.DefaultLanguage),
                    endOfCardNumber, paparaCardDisplayName);

                var package = EmailTemplates.GetCredentialOnFileBlockedMailPackage(user, dynamicContent, externalCardAcceptorList, emailTitle, new List<string> { user.Email });

                packages.Add(package);
            }

            if (!paparaCard.InternetTransactionsEnabled)
            {
                var emailTitle = StringAdapter.Format(ResourceValue(ResourceConstants.CredentialOnFileInternetDisabledCardEmailTitle, user.DefaultLanguage), paparaCardDisplayName);

                var dynamicContent = StringAdapter.Format(ResourceValue(ResourceConstants.CredentialOnFileInternetDisabledCardEmailContentIntro, user.DefaultLanguage),
                    endOfCardNumber, paparaCardDisplayName);

                var package = EmailTemplates.GetCredentialOnFileBlockedMailPackage(user, dynamicContent, externalCardAcceptorList, emailTitle, new List<string> { user.Email });

                packages.Add(package);
            }

            return packages;
        }

        public static IList<NotificationPackage> GetAlertNotification(string message, AlertLevel alertLevel)
        {
            _configurationService.ReadConfiguration("IsSmsAlertEnabled", out bool isSmsAlertEnabled);

            _configurationService.ReadConfiguration("IsEmailAlertEnabled", out bool isEmailAlertEnabled);

            _configurationService.ReadConfiguration("IsPushNotificationAlertEnabled", out bool isPushNotificationAlertEnabled);

            var packages = new List<NotificationPackage>();

            if (isSmsAlertEnabled)
            {
                packages.AddRange(BuildSmsAlerts(message, alertLevel));
            }

            if (isEmailAlertEnabled)
            {
                packages.Add(BuildEmailAlerts(message, alertLevel));
            }

            if (isPushNotificationAlertEnabled)
            {
                packages.AddRange(BuildPushNotificationAlerts(message, alertLevel));
            }

            return packages;
        }

        /// <summary>
        /// Carrefour settlement ftp upload fail notification
        /// </summary>
        public static EmailNotificationPackage FailedCarrefourSettlementFtpUpload(ServiceResult serviceResult, string to)
        {
            var subject = "Hata: Carrefour Mutabakat Dosyası Yükleme";

            return EmailTemplates.EmptyMailPackage(serviceResult.Error.Message, subject, new List<string> { to });
        }

        /// <summary>
        /// Recurring bank card deposit instruction success notification
        /// </summary>
        public static IList<NotificationPackage> SuccessRecurringBankCardDepositInstruction(User user, string period, decimal amount, string currencyCode)
        {
            var notificationPackages = new List<NotificationPackage>();

            string content, subject;

            // Notification
            if (user.CanPushMobileNotification)
            {
                subject = ResourceValue(ResourceConstants.CompletedRecurringBankCardDepositNotificationSubject, user?.DefaultLanguage);

                content = StringAdapter.Format(ResourceValue(ResourceConstants.CompletedRecurringBankCardDepositNotificationContent, user?.DefaultLanguage), period.ToLower(), amount, currencyCode);

                notificationPackages.Add(new PushNotificationPackage(content, subject, user.DeviceEndpoint, user.Id, user.DeviceToken, user.DeviceType));
            }

            // Email
            if (user.EmailConfirmed)
            {
                subject = StringAdapter.Format(ResourceValue(ResourceConstants.CompletedRecurringBankCardDepositMailSubject, user?.DefaultLanguage));

                _configurationService.ReadConfiguration("PaparaWebUrl", out string url, false);

                var dynamicContent = StringAdapter.Format(ResourceValue(ResourceConstants.CompletedRecurringBankCardDepositMailDynamicContent, user?.DefaultLanguage), period.ToLower(), amount, currencyCode);

                var package = EmailTemplates.GetRecurringBankCardDepositMailPackage(subject, user.FullName, dynamicContent, url, new List<string> { user.Email }, user?.DefaultLanguage);

                notificationPackages.Add(package);
            }

            // SMS
            if (!user.CanPushMobileNotification && !user.EmailConfirmed && !string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                content = StringAdapter.Format(ResourceValue(ResourceConstants.CompletedRecurringBankCardDepositNotificationSmsContent, user?.DefaultLanguage), period.ToLower(), amount, currencyCode);

                notificationPackages.Add(new SmsNotificationPackage(content, user.PhoneNumber));
            }

            return notificationPackages;
        }

        /// <summary>
        /// Kullanıcı şifremi unuttum bilgilendirme maili
        /// </summary>
        public static EmailNotificationPackage PaymentPartnerBalanceThreshold(string partnerName, decimal balance, decimal threshold, List<string> toEmail)
        {
            return EmailTemplates.PaymentPartnerBalanceThreshold(partnerName, balance, threshold, ResourceValue(ResourceConstants.PaymentPartnerBalanceThresholdSubject), toEmail);
        }

        /// <summary>
        /// Servis sağlayıcısı, bizim ve piyasa kur fiyatlarının karşılaştırması sonucu alarm maili
        /// </summary>
        public static EmailNotificationPackage GetFxPriceAlertEmailNoticationPackage(string serviceProviderPrice, string ourPrice, string averagePrice, string reason, string subject, List<string> to)
        {
            return EmailTemplates.GetFxPriceAlertEmailTemplate(serviceProviderPrice, ourPrice, averagePrice, reason,
                subject, to);
        }

        /// <summary>
        /// Recurring bank card deposit instruction fail notifications (ServiceResult)
        /// </summary>
        public static IList<NotificationPackage> FailedRecurringBankCardDepositInstruction(User user, ServiceResult serviceResult)
        {
            var notificationPackages = new List<NotificationPackage>();

            var smsNotification = true;

            var notificationContent = serviceResult.Error.Message;
            var smsContent = serviceResult.Error.Message;
            var emailContent = StringAdapter.Format(serviceResult.Error.Message, ResourceValue(ResourceConstants.RecurringBankCardDepositErrorMailBody, user.DefaultLanguage));

            if (user.CanPushMobileNotification)
            {
                notificationPackages.Add(new PushNotificationPackage(notificationContent, ResourceValue(ResourceConstants.RecurringRecurringBankCardDepositErrorNotificationTitle, user.DefaultLanguage), user.DeviceEndpoint, user.Id, user.DeviceToken, user.DeviceType));

                smsNotification = false;
            }

            if (user.EmailConfirmed)
            {
                notificationPackages.Add(EmailTemplates.GetRecurringBankCardDepositInstructionFailedMailPackage(user, emailContent, ResourceValue(ResourceConstants.RecurringRecurringBankCardDepositErrorNotificationTitle, user.DefaultLanguage), new List<string> { user.Email }));

                smsNotification = false;
            }

            if (smsNotification)
            {
                // Eğer kullanıcı eposta adresini onaylamadıysa ve uygulama üzerinden bildirim gönderemiyorsak, sms gönderiyoruz.
                notificationPackages.Add(new SmsNotificationPackage(smsContent, user.PhoneNumber));
            }

            return notificationPackages;
        }

        /// <summary>
        /// Send card deposit as attachment to user
        /// </summary>
        public static EmailNotificationPackage GetBankCardDepositPaymentReportMailTemplate(User user, byte[] attachmentFile, string attachmentFileName)
        {
            return EmailTemplates.GetReportMailTemplate(
                ResourceValue(ResourceConstants.ReportMailTitle, user.DefaultLanguage),
                user.FullName,
                ResourceValue(ResourceConstants.BankCardDepositPaymentReportMailContent, user.DefaultLanguage),
                new List<string> { user.Email },
                attachmentFile,
                attachmentFileName);
        }

        /// <summary>
        /// Gets merchant broker report mail template
        /// </summary>
        public static EmailNotificationPackage GetMerchantBrokerReportMailTemplate(string name, string reportMerchantName, string email, string language, byte[] attachmentFile, string attachmentFileName)
        {
            var content = string.Format(ResourceValue(ResourceConstants.MerchantBrokerReportMailContent, language), reportMerchantName);

            return EmailTemplates.GetMerchantBrokerReportMailTemplate(
                ResourceValue(ResourceConstants.MerchantBrokerReportMailTitle, language),
                name,
                content,
                new List<string> { email },
                attachmentFile,
                attachmentFileName);
        }


        private static List<NotificationPackage> BuildSmsAlerts(string message, AlertLevel alertLevel)
        {
            _configurationService.ReadConfiguration("AlertReceiverPhoneNumbers", out string alertReceiverPhoneNumbers);

            message = StringAdapter.Format(ResourceValue(ResourceConstants.MonitoringAlertSmsContent), alertLevel.GetDisplayName(), message);

            var numbers = alertReceiverPhoneNumbers.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            var packages = new List<NotificationPackage>();

            foreach (string number in numbers)
            {
                packages.Add(new SmsNotificationPackage(message, number));
            }

            return packages;
        }

        private static NotificationPackage BuildEmailAlerts(string message, AlertLevel alertLevel)
        {
            _configurationService.ReadConfiguration("AlertReceiverEmailAddresses", out string alertReceiverEmailAddresses);

            var alertSubject = StringAdapter.Format(ResourceValue(ResourceConstants.MonitoringAlertSubject), alertLevel.GetDisplayName());

            var emails = alertReceiverEmailAddresses.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            return new EmailNotificationPackage(string.Empty, message, alertSubject, emails);
        }

        private static List<NotificationPackage> BuildPushNotificationAlerts(string message, AlertLevel alertLevel)
        {
            _configurationService.ReadConfiguration("AlertReceiverDeviceEndpoints", out string alertReceiverDeviceEndpoints);

            var endpoints = alertReceiverDeviceEndpoints.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            var alertSubject = StringAdapter.Format(ResourceValue(ResourceConstants.MonitoringAlertSubject), alertLevel.GetDisplayName());

            var packages = new List<NotificationPackage>();

            foreach (string endpoint in endpoints)
            {
                packages.Add(new PushNotificationPackage(content: message, alertSubject, endpoint, null, null, DeviceType.ANDROID));
            }

            return packages;
        }


    }
}