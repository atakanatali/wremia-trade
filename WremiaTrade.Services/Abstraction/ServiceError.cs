namespace Papara.Services.Abstraction
{
    using System;
    using System.Collections.Generic;

    using Papara.Models.Entities;
    using Papara.Services.DTO;
    using Papara.Services.Resources;
    using Papara.Utilities;
    using Papara.Utilities.Extensions;

    [Serializable]
    public class ServiceError : BaseServiceResult
    {
        public static IResourceReadService ResourcesService { get; set; }

        public static IStringAdapter StringAdapter { get; set; }

        /// <summary>
        /// Do not use this public constructor.
        /// Use Create method. This constructor is public for dynamic object serialization
        /// </summary>
        public ServiceError(string message, int code) : base(message, code)
        {
        }

        public static ServiceError Create(ResourceInfo resourceInfo, int code)
        {
            var resourceValue = ResourcesService.GetResource<string>(resourceInfo.Key);

            return new ServiceError(resourceValue, code);
        }

        private static string ResourceValue(ResourceInfo resourceInfo)
        {
            return ResourcesService.GetResource<string>(resourceInfo.Key);
        }

        private static string ResourceValue(ResourceInfo resourceInfo, string languageCode)
        {
            return ResourcesService.GetResource<string>(resourceInfo.Key, languageCode);
        }

        /// <summary>
        ///     Beside of Returning null, returning NoError would be better.
        /// </summary>
        public static ServiceError NoError => null;

        #region Errors

        public static ServiceError BankTransferPending => Create(ResourceConstants.BankTransferPending, 992);

        public static ServiceError BankTransferAlreadyTransferred => Create(ResourceConstants.BankTransferAlreadyTransferred, 993);

        public static ServiceError BankTransferMustBeManuel => Create(ResourceConstants.BankTransferMustBeManuel, 994);

        public static ServiceError BankTransferError => Create(ResourceConstants.BankTransferError, 995);

        public static ServiceError CustomMessage(string errorMessage)
        {
            return new ServiceError(errorMessage, 996);
        }

        public static ServiceError CustomMessage(string errorMessage, int errorCode)
        {
            return new ServiceError(errorMessage, errorCode);
        }

        public static ServiceError ForbiddenError => Create(ResourceConstants.ForbiddenError, 997);

        public static ServiceError ModelStateError(string validationError)
        {
            return new ServiceError(validationError, 998);
        } // 998

        // DefaultError is by far the most used property. Sucessful ServiceResult is also using it.
        // Make it hard coded to avoid a huge count of redis hits
        public static ServiceError DefaultError => new ServiceError(ResourcesService.GetDefaultErrorMessage(), 999);

        public static ServiceError UserNotFound => Create(ResourceConstants.UserNotFound, 100);

        public static ServiceError MerchantNotFound => Create(ResourceConstants.MerchantNotFound, 101);

        public static ServiceError MerchantOrderNotFound => Create(ResourceConstants.MerchantOrderNotFound, 102);

        public static ServiceError MerchantOrderAlreadyExists => Create(ResourceConstants.MerchantOrderAlreadyExists, 103);

        public static ServiceError MerchantIpRestriction => Create(ResourceConstants.MerchantIpRestriction, 104);

        // Bu error koda göre işlem yapılıyor, değiştirme.
        public static ServiceError InsufficientBalance(Platform? platform = null) =>
            Create((platform == Platform.WEB ? ResourceConstants.InsufficientBalanceWithDepositReferral : ResourceConstants.InsufficientBalance), 105);

        public static ServiceError MerchantNotHavePaymentType => Create(ResourceConstants.MerchnatNotHavePaymentType, 106);

        public static ServiceError BalanceLimitExceeded(decimal limit, string currencyCode)
        {
            var message = StringAdapter.Format(ResourceValue(ResourceConstants.BalanceLimitExceeded), limit.ToMoneyString(rounded: true), currencyCode);

            return new ServiceError(message, 107);
        }
        public static ServiceError BalanceLimitExceededWithName(decimal limit, string currencyCode, string fullName, string language)
        {
            var message = StringAdapter.Format(ResourceValue(ResourceConstants.BalanceLimitExceededWithName, language), limit.ToMoneyString(rounded: true), currencyCode, fullName);

            return new ServiceError(message, 107);
        }

        /// <summary>
        /// Hata karşılaştırma için kullanılıyor.
        /// </summary>
        public static ServiceError BalanceLimitExceeded() => Create(ResourceConstants.BalanceLimitExceeded, 107);

        public static ServiceError ReceiverBalanceLimitExceeded(string userFullName, decimal limit)
        {
            var message = StringAdapter.Format(ResourceValue(ResourceConstants.BalanceLimitExceededReceiver), userFullName);

            return new ServiceError(message, 107);
        }

        public static ServiceError ReceiverBalanceLimitExceededWithName(string userFullName, decimal limit, string language)
        {
            var message = StringAdapter.Format(ResourceValue(ResourceConstants.BalanceLimitExceededReceiver, language), userFullName);

            return new ServiceError(message, 107);
        }

        /// <summary>
        /// Hata karşılaştırma için kullanılıyor.
        /// </summary>
        public static ServiceError ReceiverBalanceLimitExceeded() => Create(ResourceConstants.BalanceLimitExceeded, 107);

        public static ServiceError SameUser => Create(ResourceConstants.SameUser, 108);

        public static ServiceError HaveGotPendingRequest => Create(ResourceConstants.HaveGotPendingRequest, 109);

        public static ServiceError UserMustBeApproved => Create(ResourceConstants.ApprovedUser, 110);

        public static ServiceError TransactionLimitExceeded(bool cashDeposit, decimal limit, string currencyCode)
        {
            var message = StringAdapter.Format(cashDeposit ? ResourceValue(ResourceConstants.TransactionLimitExceededForDepositHerself) : ResourceValue(ResourceConstants.TransactionExceedLimit), limit.ToMoneyString(rounded: true), currencyCode);

            return new ServiceError(message, 111);
        } // 111

        public static ServiceError TransactionLimitExceededWithName(bool cashDeposit, decimal limit, string currencyCode, string userFullName, string language)
        {
            var message = StringAdapter.Format(cashDeposit ? ResourceValue(ResourceConstants.TransactionLimitExceededForDepositHerself) : ResourceValue(ResourceConstants.TransactionExceedLimitWithName, language), limit.ToMoneyString(rounded: true), currencyCode, userFullName);

            return new ServiceError(message, 111);
        } // 111

        public static ServiceError TransactionLimitExceeded()
        {
            return Create(ResourceConstants.TransactionExceedLimitReceiver, 111);
        }

        public static ServiceError MinDepositLimit(decimal limit, string currencyCode)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.MinDepositLimit), limit.ToMoneyString(rounded: true), currencyCode), 112);
        } // 112

        public static ServiceError MinMoneyRequestLimit(decimal limit, string currencyCode)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.MinMoneyRequestLimit), limit.ToMoneyString(rounded: true), currencyCode), 113);
        } // 113

        public static ServiceError MinSendMoneyLimit(decimal limit, CurrencyInfo currencyInfo) =>
            new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.MinSendMoneyLimit),
                                           CurrencyService.RoundDown(limit, currencyInfo.Precision),
                                           currencyInfo.PreferredDisplayCode), 114);

        public static ServiceError MinWithdrawalLimit(decimal limit, string currencyCode)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.MinWithdrawalLimit), limit.ToMoneyString(rounded: true), currencyCode), 115);
        } // 115

        public static ServiceError WaitedMoneyRequest => Create(ResourceConstants.WaitedMoneyRequest, 116);

        public static ServiceError CompanyBankAccountNotFound => Create(ResourceConstants.CompanyBankAccountNotFound, 117);

        public static ServiceError BankNotFound => Create(ResourceConstants.BankNotFound, 118);

        public static ServiceError InValidEmail => Create(ResourceConstants.FlashMessage_Error_InValidEmail_Message, 119);

        public static ServiceError BankAccountNotFound => Create(ResourceConstants.BankAccountNotFound, 120);

        public static ServiceError RequestNotFound => Create(ResourceConstants.RequestNotFound, 121);

        /// <summary>
        /// MSU dan gelen hata kodunu Servis Error' e çevirir.
        /// </summary>
        /// <param name="msuErrorCode">Msu hata kodu</param>
        /// <returns></returns>
        public static ServiceError GetMsuError(string msuErrorCode)
        {
            // Msu hata kodlarından önemli olanların listesi
            var msuErrorMessages = new Dictionary<string, string>()
            {
                {"ERR10030", ResourceValue(ResourceConstants.Msu_TransactionNotFound)},
                {"ERR10031", ResourceValue(ResourceConstants.Msu_TransactionNotRefund)},
                {"ERR10032", ResourceValue(ResourceConstants.Msu_TransactionNotRefund)},
                {"ERR10033", ResourceValue(ResourceConstants.Msu_TransactionNotCancel)},
                {"ERR10034", ResourceValue(ResourceConstants.Msu_TransactionNotFound)},
                {"ERR10041", ResourceValue(ResourceConstants.CardAlreadyExist)},
                {"ERR10045", ResourceValue(ResourceConstants.Msu_CardInfoNotVerify)},
                {"ERR10046", ResourceValue(ResourceConstants.Msu_CardInfoNotVerify)},
                {"ERR10057", ResourceValue(ResourceConstants.Msu_CardInfoNotVerify)},
                {"ERR10104", ResourceValue(ResourceConstants.Msu_CardInfoNotVerify)},
                {"ERR20005", ResourceValue(ResourceConstants.Msu_TransactionNotComplate)},
                {"ERR20008", ResourceValue(ResourceConstants.Msu_TransactionNotComplate)},
                {"ERR20011", ResourceValue(ResourceConstants.Msu_TransactionNotComplate)},
                {"ERR20012", ResourceValue(ResourceConstants.Msu_TransactionNotComplate)},
                {"ERR20027", ResourceValue(ResourceConstants.Msu_TransactionNotComplate)},
                {"ERR20033", ResourceValue(ResourceConstants.Msu_CardExpired)},
                {"ERR20051", ResourceValue(ResourceConstants.Msu_CardLimitNotEnough)},
                {"ERR20054", ResourceValue(ResourceConstants.Msu_CardExpired)},
                {"ERR20080", ResourceValue(ResourceConstants.Msu_CardInfoNotVerify)},
                {"ERR20082", ResourceValue(ResourceConstants.Msu_CardInfoNotVerify)},
                {"ERR20084", ResourceValue(ResourceConstants.Msu_CardInfoNotVerify)},
                {"ERR20099", ResourceValue(ResourceConstants.Msu_TransactionNotComplate)},
                {"ERR30001", ResourceValue(ResourceConstants.Msu_Tmx)},
                {"ERR30002", ResourceValue(ResourceConstants.Msu_TransactionNotComplate)},
                {"ERR20069", ResourceValue(ResourceConstants.Msu_CardMustBeAvailableForInternet)},
                {"ERR10016", ResourceValue(ResourceConstants.Msu_InvalidAmountInformation)},
                {"ERR10019", ResourceValue(ResourceConstants.Msu_GeneralError)},
                {"ERR10044", ResourceValue(ResourceConstants.Msu_InvalidDateFormat)},
                {"ERR10079", ResourceValue(ResourceConstants.Msu_CardBrandNotFound)},
                {"ERR10086", ResourceValue(ResourceConstants.Msu_InvalidEndDate)},
                {"ERR10088", ResourceValue(ResourceConstants.Msu_InvalidInstallmentNumber)},
                {"ERR10090", ResourceValue(ResourceConstants.Msu_GeneralError)},
                {"ERR10093", ResourceValue(ResourceConstants.Msu_CardLimitNotEnough)},
                {"ERR20014", ResourceValue(ResourceConstants.Msu_InvalidCard)},
                {"ERR20016", ResourceValue(ResourceConstants.Msu_CardLimitNotEnough)},
                {"ERR20018", ResourceValue(ResourceConstants.Msu_CardExpired)},
                {"ERR20019", ResourceValue(ResourceConstants.Msu_GeneralError)},
                {"ERR20078", ResourceValue(ResourceConstants.Msu_CardLimitNotEnough)},
                {"ERR20093", ResourceValue(ResourceConstants.Msu_CardMustBeAvailableForInternet)},
                {"ERR30005", ResourceValue(ResourceConstants.Msu_NoResponseReceivedBank)},
                {"ERR10110", ResourceValue(ResourceConstants.Msu_GeneralError)},
            };

            msuErrorCode = msuErrorCode ?? ResourceValue(ResourceConstants.Msu_CardInfoNotVerify);

            var parseSucceeded = msuErrorMessages.TryGetValue(msuErrorCode, out var message);

            var errorCode = 122;

            if (!parseSucceeded)
            {
                return DefaultError;
            }

            // Bu kodu kredi kartı ile harçlık gönderirken kartın bakiyesinin yetersiz olması durumunda
            // bilgilendirmede istediğimiz mesajı atabilmemiz için msu dan dönen bu koda göre özel bir "errorCode" ataması yaptık.
            // Bu "errorCode" u değiştirme durumumuz olursa kullanılan yerdeki koduda değiştirmemiz gerekecektir.
            if (msuErrorCode == "ERR20051")
            {
                errorCode = 999999;
            }

            return new ServiceError(message, errorCode);
        } // 122

        public static ServiceError AlreadyReversed => Create(ResourceConstants.AlreadyReversed, 123);

        public static ServiceError NotAvailable => Create(ResourceConstants.NotAvailable, 124);

        public static ServiceError CardAlreadyExist => Create(ResourceConstants.CardAlreadyExist, 125);

        public static ServiceError CardAlreadyRemoved => Create(ResourceConstants.CardAlreadyRemoved, 126);

        public static ServiceError SmsNotSend => Create(ResourceConstants.SmsNotSend, 127);

        public static ServiceError LedgerCannotBeCancelled => Create(ResourceConstants.LedgerCannotBeCancelled, 128);

        public static ServiceError UserAlreadyHasANonamePaparaCard => Create(ResourceConstants.UserAlreadyHasANonamePaparaCard, 129);

        public static ServiceError UserIsAlreadyRegistered => Create(ResourceConstants.UserIsAlreadyRegistered, 130);

        public static ServiceError WrongPaparaCardNumber => Create(ResourceConstants.WrongPaparaCardNumber, 131);

        public static ServiceError WrongSmsCode => Create(ResourceConstants.WrongSmsCode, 132);

        public static ServiceError FacebookDefaultError => Create(ResourceConstants.FacebookLoginDefaultError, 133);

        public static ServiceError ErrorConfirmEmail => Create(ResourceConstants.ErrorConfirmEmail, 134);

        public static ServiceError AlreadyConfirmedEmail => Create(ResourceConstants.AlreadyConfirmedEmail, 135);

        public static ServiceError ResetPasswordError => Create(ResourceConstants.ResetPasswordError, 136);

        public static ServiceError SameDigitsPin => Create(ResourceConstants.SameDigitsPin, 137);

        public static ServiceError RoleNotFound => Create(ResourceConstants.RolNotFound, 138);

        public static ServiceError RoleNotAdd => Create(ResourceConstants.RolNotAdd, 139);

        public static ServiceError RoleNotExists => Create(ResourceConstants.RolNotExists, 140);

        public static ServiceError WrongPassword(int remainingAccessAttempts)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.WrongPassword), remainingAccessAttempts), 141);
        }

        public static ServiceError RegisteredPhoneNumber => Create(ResourceConstants.RegisteredPhoneNumber, 142);

        public static ServiceError PaparaCardNotFound => Create(ResourceConstants.PaparaCardNotFound, 143);

        public static ServiceError UnVerifiedUser => Create(ResourceConstants.UnVerifiedUser, 144);

        public static ServiceError SendToSelfAccount => Create(ResourceConstants.SendToSelftAccount, 145);

        public static ServiceError SendSecondInvitation => Create(ResourceConstants.SendSecondInvitation, 146);

        public static ServiceError InvalidCityCode => Create(ResourceConstants.InvalidCityCode, 147);

        public static ServiceError InvalidDistrictCode => Create(ResourceConstants.InvalidDistrictCode, 148);

        public static ServiceError SendMoneyCountLimitReached(int monthlyLimit)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.SendMoneyCountLimitReached), monthlyLimit), 149);
        } // 149

        public static ServiceError InviteByMoneyTransferCountLimitReached(int monthlyLimit)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.InviteByMoneyTransferCountLimitReached), monthlyLimit), 150);
        } // 150

        public static ServiceError UserLockedOutByAdmin => Create(ResourceConstants.UserLockedOutByAdmin, 151);

        public static ServiceError WrongEmail => Create(ResourceConstants.WrongEmail, 152);

        public static ServiceError PaymentAlreadyPaid => Create(ResourceConstants.PaymentAlreadyPaid, 153);

        public static ServiceError PaparaCardIsNotAvailableForForiegnMobileNumbers => Create(ResourceConstants.PaparaCardNotAvailableAbroad, 154);

        public static ServiceError PaparaCardNotAvailableForThisOperation(string cardStatusString)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.PaparaCardNotAvailableForThisOperation), cardStatusString), 155);
        }

        /// <summary>
        /// Hata kodu karşılaştırması için kullanılıyor.
        /// </summary>
        public static ServiceError PaparaCardNotAvailableForThisOperation()
        {
            return Create(ResourceConstants.PaparaCardNotAvailableForThisOperation, 155);
        }

        public static ServiceError InvalidNeighborhoodCode => Create(ResourceConstants.InvalidNeighborhoodCode, 156);

        public static ServiceError PaparaCardOnlyOwnCardCanBeActivated => Create(ResourceConstants.PaparaCardOnlyOwnCardCanBeActivated, 157);

        public static ServiceError RolNotRemoved => Create(ResourceConstants.RolNotRemoved, 158);

        public static ServiceError InvalidIBAN => Create(ResourceConstants.InvalidIBAN, 159);

        public static ServiceError FeeExist => Create(ResourceConstants.FeeExist, 160);

        public static ServiceError FeeNotFound => Create(ResourceConstants.FeeNotFound, 161);

        public static ServiceError PaymentTypeExist => Create(ResourceConstants.PaymentTypeExist, 162);

        public static ServiceError PaymentTypeNotFound => Create(ResourceConstants.PaymentTypeNotFound, 163);

        public static ServiceError VolumeNotFound => Create(ResourceConstants.VolumeNotFound, 164);

        public static ServiceError IpNotFound => Create(ResourceConstants.IpNotFound, 165);

        public static ServiceError IpExist => Create(ResourceConstants.IpExist, 166);

        public static ServiceError InvalidCreditCardNumber => Create(ResourceConstants.InvalidCreditCardNumber, 167);

        /// <summary>
        /// This error code is used by web ui team so please don't change or overwrite.
        /// </summary>
        public static ServiceError PaymentNotFound => Create(ResourceConstants.PaymentNotFound, 168);

        public static ServiceError IBANExists => Create(ResourceConstants.IBANExists, 169);

        public static ServiceError IbanAlreadyExistAndPassive => Create(ResourceConstants.IbanAlreadyExistAndPassive, 170);

        public static ServiceError BankAccountLimitOverload(int limit) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.BankAccountLimitOverload), limit), 171);

        public static ServiceError MerchantUserIsAlreadyRegistered => Create(ResourceConstants.MerchantUserIsAlreadyRegistered, 172);

        public static ServiceError RoleNotAuthAdd => Create(ResourceConstants.RoleNotAuthAdd, 173);

        public static ServiceError LoadMoneyYourOurNotAdd => Create(ResourceConstants.LoadMoneyYourOurNotAdd, 174);


        public static ServiceError LedgerNotFound => Create(ResourceConstants.LedgerNotFound, 175);

        public static ServiceError WithdrawalRequestLimit => Create(ResourceConstants.WithdrawalRequestLimit, 176);

        public static ServiceError CardClosedToInternet => Create(ResourceConstants.PaparaCardInternetTransactionsNotAllowed, 177);

        public static ServiceError ExceedsAtmWithdrawalLimit => Create(ResourceConstants.PaparaCardExceedsAtmWithdrawalLimit, 178);

        public static ServiceError SameVerifiedAndConfirmed => Create(ResourceConstants.SameVerifiedAndConfirmed, 179);

        public static ServiceError IdentityNotVerify => Create(ResourceConstants.IdentityNotVerify, 180);

        public static ServiceError UnApprovedUser => Create(ResourceConstants.UnApprovedUser, 181);

        public static ServiceError InvalidConfirmationCode => Create(ResourceConstants.InvalidConfirmationCode, 182);

        public static ServiceError AlreadyUserUsingThisEmail => Create(ResourceConstants.AlreadyUserUsingThisEmail, 183);

        public static ServiceError CheckPasswordAreas => Create(ResourceConstants.CheckPasswordAreas, 184);

        public static ServiceError FileNotFound => Create(ResourceConstants.FileNotFound, 185);

        public static ServiceError BankTransferRequestNotFound => Create(ResourceConstants.BankTransferRequestNotFound, 186);

        public static ServiceError UserAlreadyInvited => Create(ResourceConstants.UserAlreadyInvited, 187);

        public static ServiceError MaximumInvitationAmountExceeded(string currencyCode) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.MaximumInvitationAmountExceeded), currencyCode), 188);

        public static ServiceError IdentityNumberExist => Create(ResourceConstants.IdentityNumberExist, 189);

        public static ServiceError IsExistDepositRequest => Create(ResourceConstants.IsExistDepositRequest, 190);

        public static ServiceError InvalidAccountNumber => Create(ResourceConstants.InvalidAccountNumber, 191);

        public static ServiceError InvalidPhoneNumber => Create(ResourceConstants.InvalidPhoneNumber, 192);

        public static ServiceError SmsLimitExceeded => Create(ResourceConstants.SmsLimitExceeded, 193);

        public static ServiceError UserAlreadyHasSocialAccount(string socialPlatform) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.UserAlreadyHasFacebookAccount), socialPlatform), 194);

        public static ServiceError ExistSocialAccount(string socialPlatform) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.ExistSocialAccount), socialPlatform), 195);

        public static ServiceError LinkSocialAccountWarning(string socialPlatform) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.LinkSocialAccount), socialPlatform), 196);

        public static ServiceError MoneyReceiveCountLimitExceeded => Create(ResourceConstants.MoneyReceiveCountLimitExceeded, 197);

        public static int MoneyReceiveCountLimitExceededWithNameErrorCode = 197;

        public static ServiceError MoneyReceiveCountLimitExceededWithName(string userFullName, string language)
        {
            var errorMessage = StringAdapter.Format(ResourceValue(ResourceConstants.MoneyReceiveCountLimitExceededWithName, language), userFullName);

            return new ServiceError(errorMessage, MoneyReceiveCountLimitExceededWithNameErrorCode);
        }

        public static ServiceError MinMassPaymentLimit(decimal limit) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.MinMassPaymentLimit), limit.ToMoneyString(rounded: true)), 198);

        public static ServiceError ApproveUserAboveAge => Create(ResourceConstants.ApproveUserAboveAge, 199);

        public static ServiceError ApproveUserBelowAge => Create(ResourceConstants.ApproveUserBelowAge, 200);

        public static ServiceError TFACodeAttemptLimitExceeded => Create(ResourceConstants.TFACodeAttemptLimitExceeded, 201);

        public static ServiceError MerchantApplicationNotFound => Create(ResourceConstants.MerchantApplicationNotFound, 202);

        public static ServiceError UserLocked => Create(ResourceConstants.UserLocked, 203);

        public static ServiceError UserLockedWithName(string userFullName, string language) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.UserLockedWithName, language), userFullName), 203);

        public static ServiceError NotBelongIbanToUser => Create(ResourceConstants.NotBelongIbanToUser, 204);

        public static ServiceError IbanCouldNotVerified => Create(ResourceConstants.IbanCouldNotVerified, 205);

        public static ServiceError EmailNotExistOnFacebook => Create(ResourceConstants.EmailNotExistOnFacebook, 206);

        public static ServiceError BankAccountCodeExist => Create(ResourceConstants.BankAccountCodeExist, 207);

        public static ServiceError BankAccountCodeAlreadyExistAndPassive => Create(ResourceConstants.BankAccountCodeAlreadyExistAndPassive, 208);

        public static ServiceError TcknMismatchWithBank => Create(ResourceConstants.TcknMismatchWithBank, 209);

        public static ServiceError OnlyCanChangeUserTypeFromApprovedToVerified => Create(ResourceConstants.OnlyCanChangeUserTypeFromApprovedToVerified, 210);

        public static ServiceError UpdateMobileAppMessage => Create(ResourceConstants.UpdateMobileAppMessage, 211);

        public static ServiceError BankNotinCompanyBankAccount => Create(ResourceConstants.BankNotinCompanyBankAccount, 212);

        public static ServiceError DateIsNotValid => Create(ResourceConstants.DateIsNotValid, 213);

        public static ServiceError MassPaymentNotFound => Create(ResourceConstants.MassPaymentNotFound, 214);

        public static ServiceError CashDepositAlreadyExists => Create(ResourceConstants.CashDepositAlreadyExists, 215);

        public static ServiceError CashDepositNotFound => Create(ResourceConstants.CashDepositNotFound, 216);

        public static ServiceError PaparaCardPinNotChanged => Create(ResourceConstants.PaparaCardPinNotChanged, 217);

        public static ServiceError UserMustBeVerifiedToWithdrawFromAtm =>
            Create(ResourceConstants.UserMustBeVerifiedToWithdrawFromAtm, 218);

        public static ServiceError PaparaCardMCCIsNotAllowed(string mcc) =>
            new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.PaparaCardMCCIsNotAllowed), mcc), 219);

        public static ServiceError ResetPinLimitInHour => Create(ResourceConstants.ResetPinLimitInOneMinute, 220);

        public static ServiceError InvitationBonusLimitReached(int monthlyLimit)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.InvitationBonusLimitReached), monthlyLimit), 221);
        }

        public static ServiceError UserPhoneNumberNotConfirmed => Create(ResourceConstants.UserPhoneNumberNotConfirmed, 222);

        public static ServiceError PhoneNumberNotFromTurkey => Create(ResourceConstants.PhoneNumberNotFromTurkey, 223);

        public static ServiceError InvitationBonusAlreadyUsed => Create(ResourceConstants.InvitationBonusAlreadyUsed, 224);

        public static ServiceError InviteByMoneyTransferIsNotNew => Create(ResourceConstants.InviteByMoneyTransferIsNotNew, 225);

        public static ServiceError PaparaCardIsNotAvailableForDeposit => Create(ResourceConstants.PaparaCardIsNotAvailableForDeposit, 226);

        public static ServiceError BankTransferRequestExits => Create(ResourceConstants.BankTransferRequestExits, 229);

        public static ServiceError DepositProviderNotFound => Create(ResourceConstants.DepositProviderNotFound, 230);

        // Buna herhangi bir hata mesajı atamadık çünkü, kod kısmında handle ettiğimiz için sadece hata kodunu kullanıyoruz kullanıcıya herhangi bir mesaj dönmüyoruz.
        public static ServiceError ExpiredKkbToken => new ServiceError("", 231);

        public static ServiceError ExceedUserMerchantVolume => Create(ResourceConstants.ExceedUserMerchantVolume, 232);

        public static ServiceError EntyTypeAlreadyExist => Create(ResourceConstants.EntyTypeAlreadyExist, 233);

        public static ServiceError UsersCanHaveOnlyOneLiveBlackCard => Create(ResourceConstants.UsersCanHaveOnlyOneLiveBlackCard, 235);

        public static ServiceError RemoteServerError => Create(ResourceConstants.RemoteServerError, 236);

        public static ServiceError TcknMismatch => Create(ResourceConstants.TcknMismatch, 237);

        public static ServiceError PositionNotFound => Create(ResourceConstants.PositionNotFound, 238);

        public static ServiceError LimitNotFound => Create(ResourceConstants.LimitNotFound, 239);

        public static ServiceError NotAuthorizedOnthisCard => Create(ResourceConstants.NotAuthorizedOnthisCard, 241);

        public static ServiceError InsufficientLimit => Create(ResourceConstants.InsufficientLimit, 242);

        public static ServiceError TcknMismatchPaparaCard => Create(ResourceConstants.TcknMismatchPaparaCard, 243);

        public static ServiceError IncorrectFileType => Create(ResourceConstants.IncorrectFileType, 244);

        public static ServiceError InternalPaparaCardAlreadyExists => Create(ResourceConstants.InternalPaparaCarAlreadyExist, 245);

        public static ServiceError HashAuthorizationFailed => Create(ResourceConstants.HashAuthorizationFailed, 246);

        public static ServiceError MerchantLocked => Create(ResourceConstants.MerchantLocked, 247);

        public static ServiceError RequestTimeOut => Create(ResourceConstants.RequestTimeOut, 248);

        public static ServiceError UserDoesNotHaveEduCard => Create(ResourceConstants.UserDoesNotHaveEduCard, 249);

        public static ServiceError ParentExceedsStudentLimit => Create(ResourceConstants.ParentExceedsStudentLimit, 250);

        public static ServiceError PocketMoneyNotFound => Create(ResourceConstants.PocketMoneyNotFound, 251);

        public static ServiceError StudentUserAlreadyPocketMoneyInstruction => Create(ResourceConstants.ParentAlreadyHasPocketMoneyInstructionForStudent, 252);

        public static ServiceError MaxPocketMoneyLimit(decimal limit, string currencyCode)
        {
            var message = StringAdapter.Format(ResourceValue(ResourceConstants.MaxPocketMoneyLimit), limit.ToMoneyString(rounded: true), currencyCode);

            return new ServiceError(message, 253);
        }

        public static ServiceError MerchantDoesNotHaveThisRole => Create(ResourceConstants.MerchantDoesNotHaveThisRole, 254);

        public static ServiceError StudentUserAlreadyAdded => Create(ResourceConstants.StudentUserAlreadyAdded, 255);

        public static ServiceError StudentIsNotAddedToParentAccount => Create(ResourceConstants.StudentIsNotAddedToParentAccount, 256);

        public static ServiceError SameUserStudents => Create(ResourceConstants.SameUserStudents, 257);

        public static ServiceError StudentUserAlreadyRemoved => Create(ResourceConstants.StudentUserAlreadyRemoved, 258);

        public static ServiceError AlreadyActiveCard => Create(ResourceConstants.AlreadyActiveCard, 264);

        public static ServiceError CampaignNotFound => Create(ResourceConstants.CampaignNotFound, 265);

        public static ServiceError NotFound => Create(ResourceConstants.NotFound, 266);

        public static ServiceError DuplicateRefundLine => Create(ResourceConstants.DuplicateRefundLine, 267);

        public static ServiceError MonthlyLimitForStudent => Create(ResourceConstants.MonthlyLimitForStudent, 269);

        public static ServiceError ExceedLimitPocketMoneyByCard(decimal remainingLimit)
        {
            var message = StringAdapter.Format(ResourceValue(ResourceConstants.ExceedLimitReceiverPocketMoneyByCard), remainingLimit.ToMoneyString(rounded: true));

            return new ServiceError(message, 270);
        }

        public static ServiceError ExceedLimitPocketMoneyByCardForSender(decimal remainingLimit)
        {
            var message = StringAdapter.Format(ResourceValue(ResourceConstants.ExceedLimitPocketMoneyByCardForSender), remainingLimit.ToMoneyString(rounded: true));

            return new ServiceError(message, 271);
        }

        public static ServiceError CardTypeNotAllowedForWithdrawal(string cardType)
        {
            var message = StringAdapter.Format(ResourceValue(ResourceConstants.CardTypeNotAllowedForWithdrawal), cardType);

            return new ServiceError(message, 272);
        }

        public static ServiceError PendingDepositRefundNotFound => Create(ResourceConstants.PendingDepositRefundNotFound, 273);

        // NOT: Buradaki kod önemli. 274 e göre işlem yapıyoruz, eğer bu kodu değiştirmek zorunda kalırsak, bu alana bağlı olan
        // UI ve varsa mobildeki alanlarda ona göre değiştirilmedi.
        public static ServiceError UserIpAddressChanged => Create(ResourceConstants.UserIpAddressChanged, 274);

        public static ServiceError NotFoundPendingRefund => Create(ResourceConstants.NotFoundPendingRefund, 275);

        public static ServiceError UserFullNameNecessary => Create(ResourceConstants.UserFullNameNecessary, 276);

        public static ServiceError IdentityNumberNecessary => Create(ResourceConstants.IdentityNumberNecessary, 277);

        public static ServiceError BankDepositTransactionExits => Create(ResourceConstants.BankDepositTransactionExits, 278);

        public static ServiceError BankDepositRefundOnlyManual => Create(ResourceConstants.BankDepositRefundOnlyManual, 279);

        public static ServiceError BankWithdrawalDisabled => Create(ResourceConstants.BankWithdrawalDisabled, 280);

        public static ServiceError RegisterNewPin => Create(ResourceConstants.RegisterNewPin, 281);

        public static ServiceError CashDepositProvisionAlreadyComplete(long id)
        {
            var message = StringAdapter.Format(ResourceValue(ResourceConstants.CashDepositProvisionAlreadyComplete), id);

            return new ServiceError(message, 283);
        }

        public static ServiceError CashDepositProvisionAlreadyCancel => Create(ResourceConstants.CashDepositProvisionAlreadyCancel, 284);

        public static ServiceError EftNotAllowedForThisBank => Create(ResourceConstants.EftNotAllowedForThisBank, 285);

        public static ServiceError EmailTypoError(string didYouMean)
        {
            var message = StringAdapter.Format(ResourceValue(ResourceConstants.EmailTypoMessage), didYouMean);

            return new ServiceError(message, 286);
        }

        public static ServiceError EmailSyntaxError => Create(ResourceConstants.EmailSyntaxError, 287);

        public static ServiceError EmailDomainBlackListed(string blackListedDomain)
        {
            var message = StringAdapter.Format(ResourceValue(ResourceConstants.EmailDomainBlackListed), blackListedDomain);

            return new ServiceError(message, 288);
        }

        public static ServiceError NumberIsFromABlacklistedCountry(string countryName)
        {
            var message = StringAdapter.Format(ResourceValue(ResourceConstants.NumberIsFromABlacklistedCountry), countryName);

            return new ServiceError(message, 289);
        }

        public static ServiceError VirtualOrUnreachablePhoneNumber => Create(ResourceConstants.VirtualOrUnreachablePhoneNumber, 290);

        public static ServiceError DescriptionNecessary => Create(ResourceConstants.DescriptionNecessary, 291);

        public static ServiceError KKTCIbanAreNotAllowed => Create(ResourceConstants.KKTCIbanAreNotAllowed, 292);

        public static ServiceError InvalidHumanVerificationToken => Create(ResourceConstants.InvalidHumanVerificationToken, 293);

        public static ServiceError NoBankDepositUserExceededLimit => Create(ResourceConstants.NoBankDepositUserExceededLimit, 294);

        public static ServiceError PaparaCardCityIsNotAllowed(string city) =>
            new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.PaparaCardCityIsNotAllowed), city), 295);

        public static ServiceError ChangedAccessToken => Create(ResourceConstants.ChangedAccessToken, 296);

        public static ServiceError UserExceedsNumberOfSavedCardsLimit(int limit)
        {
            var message = StringAdapter.Format(ResourceValue(ResourceConstants.ExceededNumberOfSavedCardsLimit), limit);

            return new ServiceError(message, 297);
        }

        public static ServiceError NoBankDepositUserCannotPayToCryptoMerchant => Create(ResourceConstants.NoBankDepositUserCannotPayToCryptoMerchant, 298);

        public static ServiceError UserExceedsLifetimeMaxPaparaCardLimit(int limit)
        {
            var message = StringAdapter.Format(ResourceValue(ResourceConstants.UserExceedsLifetimeMaxPaparaCardLimit), limit);

            return new ServiceError(message, 299);
        }

        public static ServiceError StudentExceedsParentLimit => Create(ResourceConstants.StudentExceedsParentLimit, 300);

        public static ServiceError TcknRequired => Create(ResourceConstants.TcknRequired, 301);

        public static ServiceError AlreadyUserUsingThisPhoneNumber => Create(ResourceConstants.AlreadyUserUsingThisPhoneNumber, 302);

        public static ServiceError SamePhoneNumberCannotBeUsedMoreThanTwice => Create(ResourceConstants.SamePhoneNumberCannotBeUsedMoreThanTwice, 303);

        public static ServiceError CouldNotFindUserWithPhoneNumber => Create(ResourceConstants.CouldNotFindUserWithPhoneNumber, 304);

        public static ServiceError CouldNotFindUserWithEmail => Create(ResourceConstants.CouldNotFindUserWithPhoneEmail, 305);

        public static ServiceError CouldNotFindUserWithPaparaAccountNumber => Create(ResourceConstants.CouldNotFindUserWithPaparaAccountNumber, 306);

        public static ServiceError CouldNotVerifyCaptcha => Create(ResourceConstants.CouldNotVerifyCaptcha, 307);

        public static ServiceError StudentDidNotApproveParent => Create(ResourceConstants.StudentDidNotApproveParent, 308);

        public static ServiceError StudentCannotAddStudent => Create(ResourceConstants.StudentCannotAddStudent, 309);

        public static ServiceError ParentCannotAddEduCard => Create(ResourceConstants.ParentCannotAddEduCard, 310);

        public static ServiceError CannotRemoveStudentWithActivePocketMoneyInstruction => Create(ResourceConstants.CannotRemoveStudentWithActivePocketMoneyInstruction, 311);

        public static ServiceError BankTransactionIdExist => Create(ResourceConstants.BankTransactionIdExist, 313);

        public static ServiceError BankTransactionNotFound => Create(ResourceConstants.BankTransactionNotFound, 314);

        public static ServiceError OnlyPermittedParentCanSetInternetSpendingForEduCard => Create(ResourceConstants.OnlyPermittedParentCanSetInternetSpendingForEduCard, 315);

        public static ServiceError PushNotificationCouldNotSend => Create(ResourceConstants.PushNotificationCouldNotSend, 316);

        public static ServiceError OnlyAllowedTerminalSpendingCard => Create(ResourceConstants.OnlyAllowedTerminalSpendingCard, 317);

        public static ServiceError UserHasNotPhoneNumber => Create(ResourceConstants.UserHasNotPhoneNumber, 318);

        public static ServiceError UserMustChangePassword => Create(ResourceConstants.UserMustChangePassword, 319);


        public static ServiceError WrongCvv2Code => Create(ResourceConstants.WrongCvv2Code, 320);

        public static ServiceError CashDepositProvisionCountLimitReached(int monthlyLimit)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.CashDepositProvisionCountLimitReached), monthlyLimit), 321);
        }

        public static ServiceError CashDepositProvisionDailyCountLimitReached(int limit)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.CashDepositProvisionDailyCountLimitReached), limit), 322);
        }

        public static ServiceError InSufficientSearchCriteria => Create(ResourceConstants.YouMustEnterAtLeastOneCriteriaToSearch, 323);

        public static ServiceError SameIpAddressValidLoginLimitExceeded => Create(ResourceConstants.SameIpAddressValidLoginLimitExceeded, 324);

        public static ServiceError ReLogin => Create(ResourceConstants.ReLogin, 325);

        public static ServiceError UsersPhoneInRoamingCantSetNumber => Create(ResourceConstants.UsersPhoneInRoaming, 326);

        public static ServiceError UsersPhoneInRoamingCantSendMoney => Create(ResourceConstants.UsersPhoneInRoamingCantSendMoney, 327);

        public static ServiceError ExceededRequestLimit => Create(ResourceConstants.ExceededRequestLimit, 328);

        public static ServiceError InvalidNaming => Create(ResourceConstants.InvalidNaming, 329);

        public static ServiceError ManuelTransactionLimitExceeded(decimal manuelTransactionLimit) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.ManuelTransactionLimitExceeded), manuelTransactionLimit), 330);

        public static ServiceError PhoneNumberNotFound => Create(ResourceConstants.PhoneNumberNotFound, 331);

        public static ServiceError UnableToRemovePhoneNumber => Create(ResourceConstants.UnableToRemovePhoneNumber, 332);

        public static ServiceError YouHaveAlreadyWaitingMassPaymentFile => Create(ResourceConstants.YouHaveWaitingMassPaymentFile, 333);

        // TODO : will be removed
        public static ServiceError UserMustBeUnapproved => Create(ResourceConstants.UserMustBeUnapproved, 334);

        public static ServiceError CardHasPocketMoneyInstruction => Create(ResourceConstants.CardHasPocketMoneyInstruction, 335);

        public static ServiceError ConfirmedOrApprovedAccountsTcknCannotBeRemoved => Create(ResourceConstants.ConfirmedOrApproovedAccountsTcknCannotBeRemoved, 336);

        public static ServiceError IdentityNumberNotFound => Create(ResourceConstants.IdentityNumberNotFound, 337);

        public static ServiceError VirtualCardNotExists => Create(ResourceConstants.VirtualCardNotExists, 338);

        public static ServiceError MassPaymentIncorrectFileFormat => Create(ResourceConstants.MassPaymentIncorrectFileFormat, 339);

        /// <summary>
        /// This error code is used by mobile teams so please don't change or overwrite.
        /// </summary>
        public static ServiceError MailIsAlreadyUsing => Create(ResourceConstants.MailIsAlreadyUsing, 340);

        /// <summary>
        /// This error code is used by mobile teams so please don't change or overwrite.
        /// </summary>
        public static ServiceError PhoneNumberIsAlreadyUsing => Create(ResourceConstants.PhoneNumberIsAlreadyUsing, 341);

        public static ServiceError UserDoesNotHaveAtmWithdrawalRole => Create(ResourceConstants.UserDoesNotHaveAtmWithdrawalRole, 342);

        public static ServiceError InvalidIdentityNumberOrPhoneNumber => Create(ResourceConstants.InvalidIdentityNumberOrPhoneNumber, 343);

        public static ServiceError UserExceedsMonthlyVirtualPaparaCardLimit(int maxAllowedVirtualPaparaCardCount)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.UserExceedsMonthlyVirtualPaparaCardLimit), maxAllowedVirtualPaparaCardCount), 344);
        }

        public static ServiceError UserAlreadyHasAnEduCard => Create(ResourceConstants.UserAlreadyHasAnEduCard, 347);

        public static ServiceError UserCantActivateAnEduCard => Create(ResourceConstants.UserCantActivateAnEduCard, 345);

        public static ServiceError UserAlreadyHasAVirtualPaparaCard => Create(ResourceConstants.UserAlreadyHasAVirtualPaparaCard, 348);

        public static ServiceError InvalidIslemCodeParameterOnCardTransactionRequest => Create(ResourceConstants.InvalidIslemCodeParameterOnCardTransactionRequest, 349);

        public static ServiceError BankWebServicesNotFound => Create(ResourceConstants.BankWebServicesNotFound, 351);

        public static ServiceError PaparaCardIsNotActive => Create(ResourceConstants.PaparaCardIsNotActive, 352);

        public static ServiceError PaparaCardIsNotAvailableForActivation(string cardStatusString)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.PaparaCardIsNotAvailableForActivation), cardStatusString), 353);
        }

        public static ServiceError UserDoesNotHavePosTransactionRole => Create(ResourceConstants.UserDoesNotHavePosTransactionRole, 354);

        public static ServiceError PaparaEduCardMCCIsNotAllowed(string mcc) =>
            new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.PaparaEduCardMCCIsNotAllowed), mcc), 355);

        public static ServiceError UnsafeImage => Create(ResourceConstants.UnsafeImage, 356);

        public static ServiceError IllegalTextInImage => Create(ResourceConstants.IllegalTextInImage, 357);

        public static ServiceError NoFaceInImage => Create(ResourceConstants.NoFaceInImage, 358);

        public static ServiceError InvalidImageFormat => Create(ResourceConstants.InvalidImageFormat, 359);

        public static ServiceError MaxAcceptableSizeOfUploadedImage(int size) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.MaxAcceptableSizeOfUploadedImage), size), 360);

        public static ServiceError ShouldLinkSocialAccountBeforeGetImage(string socialPlatform) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.ShouldLinkSocialAccountBeforeGetImage), socialPlatform), 361);

        public static ServiceError RequestToSelftAccount => Create(ResourceConstants.RequestToSelftAccount, 362);

        public static ServiceError RequestMoneyMonthlyCountLimitReached(int monthlyLimit)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.RequestMoneyMonthlyCountLimitReached), monthlyLimit), 364);
        }

        public static ServiceError RequestMoneyDailyCountLimitReached(int dailyLimit)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.RequestMoneyDailyCountLimitReached), dailyLimit), 365);
        }

        public static ServiceError TooManyContacts => Create(ResourceConstants.TooManyContacts, 366);

        public static ServiceError ThisCardNotPermittedToFeeRefund => Create(ResourceConstants.ThisCardNotPermittedToFeeRefund, 367);

        public static ServiceError UserExceededCardFeeRefundLimit => Create(ResourceConstants.UserExceededCardFeeRefundLimit, 368);

        public static ServiceError LedgerAmountCannotBeZero => Create(ResourceConstants.LedgerAmountCannotBeZero, 369);

        public static ServiceError UserWithNoPhoneNumberCannotUsePaparaCard => Create(ResourceConstants.UserWithNoPhoneNumberCannotUsePaparaCard, 370);

        public static ServiceError PaparaCardIsCancelled => Create(ResourceConstants.PaparaCardIsCancelled, 371);

        public static ServiceError BankTransactionDisabled => Create(ResourceConstants.BankTransactionDisabled, 372);

        public static ServiceError SocialAccountConnectionError => Create(ResourceConstants.SocialAccountConnectionError, 373);

        public static ServiceError InvalidSsoSignature => Create(ResourceConstants.InvalidSsoSignature, 374);

        public static ServiceError EmailMustBeConfirmed => Create(ResourceConstants.EmailMustBeConfirmed, 375);

        public static ServiceError InvalidMifareFeedBackLine => Create(ResourceConstants.InvalidMifareFeedBackLine, 376);

        public static ServiceError OriginalNotFound => Create(ResourceConstants.OriginalNotFound, 378);


        /// <summary>
        /// TransactionLimitExceeded error hem gönderici hem de alıcı için çalışıyordu.
        /// Fakat alıcının limiti dolduğunda alıcıya notification göndermeye karar verdiğimizden dolayı ayrılması gerekti.
        /// </summary>
        public static ServiceError ReceiverTransactionLimitExceeded(bool cashDeposit, decimal limit, string currencyCode)
        {
            string message = cashDeposit ?
                StringAdapter.Format(ResourceValue(ResourceConstants.TransactionLimitExceededForDepositHerself), limit.ToMoneyString(rounded: true), currencyCode) :
                StringAdapter.Format(ResourceValue(ResourceConstants.TransactionExceedLimitReceiver));

            return new ServiceError(message, 377);
        } // 377

        /// <summary>
        /// TransactionLimitExceeded error hem gönderici hem de alıcı için çalışıyordu.
        /// Fakat alıcının limiti dolduğunda alıcıya notification göndermeye karar verdiğimizden dolayı ayrılması gerekti.
        /// </summary>
        public static ServiceError ReceiverTransactionLimitExceededWithName(bool cashDeposit, decimal limit, string currencyCode, string fullName, string language)
        {
            string message = cashDeposit ?
                StringAdapter.Format(ResourceValue(ResourceConstants.TransactionLimitExceededForDepositHerself), limit.ToMoneyString(rounded: true), currencyCode) :
                StringAdapter.Format(ResourceValue(ResourceConstants.TransactionExceedLimitReceiverWithName, language), fullName);

            return new ServiceError(message, 377);
        } // 377

        public static ServiceError ReceiverTransactionLimitExceeded()
        {
            return Create(ResourceConstants.TransactionExceedLimitReceiver, 377);
        }


        public static ServiceError DeviceIdNotMatched => Create(ResourceConstants.DeviceIdNotMatched, 379);

        public static ServiceError CouldNotFoundCashbackCondition => Create(ResourceConstants.CouldNotFoundCashbackCondition, 380);

        public static ServiceError OnlyAppliedToAssignedUsers => Create(ResourceConstants.OnlyAppliedToAssignedUsers, 381);

        public static ServiceError CashbackConditionAlreadyAssignedToUser => Create(ResourceConstants.CashbackConditionAlreadyAssignedToUser, 382);

        public static ServiceError ExternalCardAcceptorCouldNotFound => Create(ResourceConstants.ExternalCardAcceptorCouldNotFound, 383);

        public static ServiceError BalanceAlreadyExistOnThisUser => Create(ResourceConstants.BalanceAlreadyExistOnThisUser, 384);

        public static ServiceError PositiveLockedBalanceAmount => Create(ResourceConstants.PositiveLockedBalanceAmount, 385);

        public static ServiceError BalanceNotExists(Currency currency) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.BalanceNotExists), currency.ToString()), 386);

        public static ServiceError ExceedsAtmWithdrawalMonthlyLimit => Create(ResourceConstants.ExceedsAtmWithdrawalMonthlyLimit, 387);

        public static ServiceError StudentBelowAge => Create(ResourceConstants.StudentBelowAge, 388);

        public static ServiceError ApproveStudentBelowAge => Create(ResourceConstants.ApproveStudentBelowAge, 389);

        public static ServiceError PocketMoneyCountLimitReached => Create(ResourceConstants.PocketMoneyCountLimitReached, 390);

        public static ServiceError NegativeBalance => Create(ResourceConstants.NegativeBalance, 391);

        public static ServiceError OnlyBlackCardCanActivateWithBarcode => Create(ResourceConstants.OnlyBlackCardCanActivateWithBarcode, 392);

        public static ServiceError AnErrorOccurredOnCardApplication(bool feeRefunded)
        {
            var message = feeRefunded
                ? ResourceValue(ResourceConstants.AnErrorOccurredOnCardApplicationAndFeeRefunded)
                : ResourceValue(ResourceConstants.AnErrorOccurredOnCardApplication);

            return new ServiceError(message, 393);
        }

        public static ServiceError MobileTokenNotResolved => Create(ResourceConstants.MobileTokenNotResolved, 394);

        public static ServiceError UserNotPermitted(string roleString)
        {
            var message = StringAdapter.Format(ResourceValue(ResourceConstants.UserNotPermitted), roleString);

            return new ServiceError(message, 395);
        }

        public static ServiceError MaxSendMoneyLimit(decimal limit, CurrencyInfo currencyInfo) =>
            new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.MaxSendMoneyLimit),
                                           CurrencyService.RoundDown(limit, currencyInfo.Precision),
                                           currencyInfo.PreferredDisplayCode), 396);

        public static ServiceError MaxMoneyRequestLimit(decimal limit, string currencyCode)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.MaxMoneyRequestLimit), limit.ToMoneyString(rounded: true), currencyCode), 397);
        }

        public static ServiceError ReceiverUserMustBeApprovedForCurrencyTransfer => Create(ResourceConstants.ReceiverUserMustBeApprovedForCurrencyTransfer, 398);

        public static ServiceError ReceiverUserMustBeApprovedForCurrencyTransferWithName(string userFullName, string language) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.ReceiverUserMustBeApprovedForCurrencyTransferWithName, language), userFullName), 398);

        public static ServiceError XSSError(string field) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.XSSError), field), 399);

        public static ServiceError BtcWithdrawalTransactionExceedLimit(decimal limit)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.BtcWithdrawalTransactionExceedLimit), limit.ToMoneyString(rounded: true)), 400);
        }

        public static ServiceError MonthlyExternalCardExceptorTransactionLimit(string cardExceptorName, string limit)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.MonthlyExternalCardExceptorTransactionLimit), cardExceptorName, limit), 401);
        }

        public static ServiceError TransactionAmountExceedCardLimit => Create(ResourceConstants.TransactionAmountExceedCardLimit, 402);

        public static ServiceError CardLimitExceedUserBalanceLimit => Create(ResourceConstants.CardLimitExceedUserBalanceLimit, 403);

        public static ServiceError AllPhoneNumbersMustBeRegistered => Create(ResourceConstants.AllPhoneNumbersMustBeRegistered, 404);

        public static ServiceError MonthlySplitRequestLimitExceeded => Create(ResourceConstants.MonthlySplitRequestLimitExceeded, 405);

        public static ServiceError SplitUserCountExceeded => Create(ResourceConstants.SplitUserCountExceeded, 406);

        public static ServiceError SplitUserCountMustBeAtLeastOne => Create(ResourceConstants.SplitUserCountMustBeAtLeastOne, 407);

        public static ServiceError SplitLedgerIsNotPosTransaction => Create(ResourceConstants.SplitLedgerIsNotPosTransaction, 408);

        public static ServiceError SplitLedgerMustBelongToSenderUser => Create(ResourceConstants.SplitLedgerMustBelongToSenderUser, 409);

        public static ServiceError LedgerIsAlreadySplitted => Create(ResourceConstants.LedgerIsAlreadySplitted, 410);

        public static ServiceError RolesNotRemoved => Create(ResourceConstants.RolesNotRemoved, 411);

        public static ServiceError UserMustHaveBankDeposit => Create(ResourceConstants.UserMustHaveBankDeposit, 412);

        public static ServiceError UserMustHaveBankDepositForCryptoOperations => Create(ResourceConstants.UserMustHaveBankDepositForCryptoOperations, 413);

        public static ServiceError EditedSplitAmountSumCannotExceedLedgerSum => Create(ResourceConstants.EditedSplitAmountSumCannotExceedLedgerSum, 414);

        public static ServiceError SplitAmountSumDoesNotMatchLedgerAmount => Create(ResourceConstants.SplitAmountSumDoesNotMatchLedgerAmount, 415);

        public static ServiceError UnableToUploadFileToFtp(string fileName, string path)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.UnableToUploadFileToFtp), fileName, path), 416);

        public static ServiceError SenderExceedsMonthlyTransactionLimit(decimal limit, string currencyDisplayCode)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.TransactionExceedLimitSender), limit, currencyDisplayCode), 417);

        public static ServiceError InvitationCurrencyNotValid => Create(ResourceConstants.InvitationCurrencyNotValid, 418);

        public static ServiceError ReportAlreadyExist => Create(ResourceConstants.ReportAlreadyExist, 419);

        public static ServiceError IbanAlreadyAddedToUser => Create(ResourceConstants.IbanAlreadyAddedToUser, 420);

        public static ServiceError BranchCodeOrAccountCodeIsNull => Create(ResourceConstants.BranchCodeOrAccountCodeCannotBeNull, 421);

        public static ServiceError PendingExcelDepositFileExists => Create(ResourceConstants.PendingExcelDepositFileExists, 422);

        public static ServiceError SameDepositExcelFileExists => Create(ResourceConstants.SameDepositExcelFileExists, 423);

        public static ServiceError ExcelDepositReadFileError => Create(ResourceConstants.ExcelDepositReadFileError, 424);

        public static ServiceError ExcelDepositTransactionLimitExceeded(int limit)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.ExcelDepositTransactionLimitExceeded), limit), 425);

        public static ServiceError ExcelDepositFormatErrorWithMessage(string errorMessage)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.ExcelDepositFormatErrorWithMessage), errorMessage), 426);

        public static ServiceError ExcelDepositFormatError => Create(ResourceConstants.ExcelDepositFormatError, 427);

        public static ServiceError ExcelDepositReadTransactionsError => Create(ResourceConstants.ExcelDepositReadTransactionsError, 428);

        public static ServiceError UnApprovedUserCanNotCardApplication => Create(ResourceConstants.UnApprovedUserCanNotCardApplication, 429);

        public static ServiceError RolePendingToAdd => Create(ResourceConstants.RolePendingToAdd, 430);

        public static ServiceError UserCardIsNotExist => Create(ResourceConstants.UserCardIsNotExist, 431);

        public static ServiceError CardTokenCannotBeNull => Create(ResourceConstants.CardTokenCannotBeNull, 432);

        public static ServiceError ReceiverNeedsToBeCommercial => Create(ResourceConstants.ReceiverNeedsToBeCommercial, 433);

        public static ServiceError ReceiverUserMustBeVerified => Create(ResourceConstants.ReceiverUserMustBeVerified, 434);

        public static ServiceError UserNotRegistered => Create(ResourceConstants.UserNotRegistered, 435);

        public static ServiceError ResourceAlreadyExists => Create(ResourceConstants.ResourceAlreadyExists, 436);

        public static ServiceError ResourceDoesNotExist => Create(ResourceConstants.ResourceDoesNotExist, 437);

        public static ServiceError InvalidResourceKey(string allowedCharacters) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.InvalidResourceKey), allowedCharacters), 438);

        public static ServiceError InvalidResourceLanguage(string allowedLanguages) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.InvalidResourceLanguage), allowedLanguages), 439);

        public static ServiceError IncompatibleResourceTargetAndType(string target, string type) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.IncompatibleResourceTargetAndType), target, type), 440);

        public static ServiceError InvalidResourceKeyLength(int minLength, int maxLength) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.InvalidResourceKeyLength), minLength, maxLength), 441);

        public static ServiceError InvalidResourceStringValueLength(int maxLength) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.InvalidResourceStringValueLength), maxLength), 442);

        public static ServiceError InvalidResourceBinaryValueLength(int maxLength) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.InvalidResourceBinaryValueLength), maxLength), 443);

        public static ServiceError RoleNotRemove => Create(ResourceConstants.RoleNotRemove, 444);

        public static ServiceError RolePendingToRemove => Create(ResourceConstants.RolePendingToRemove, 445);


        public static ServiceError BalanceAlreadyExistOnThisMerchant => Create(ResourceConstants.BalanceAlreadyExistOnThisMerchant, 446);

        public static ServiceError MoneyTransferInstructionNotFound => Create(ResourceConstants.MoneyTransferInstructionNotFound, 447);

        public static ServiceError UserAlreadyHasMoneyTransferInstruction => Create(ResourceConstants.UserAlreadyHasMoneyTransferInstruction, 448);

        public static ServiceError CommercialLinkAlreadyExists => Create(ResourceConstants.CommercialLinkAlreadyExists, 449);

        public static ServiceError CommercialApplicationRequireDescription => Create(ResourceConstants.CommercialApplicationRequireDescription, 450);

        public static ServiceError UserMustBeVerified => Create(ResourceConstants.UserMustBeVerified, 451);

        public static ServiceError UserIsAlreadyCommercial => Create(ResourceConstants.UserIsAlreadyCommercial, 452);

        public static ServiceError UserAlreadyHasActiveCommercialAccountApplication => Create(ResourceConstants.UserAlreadyHasActiveCommercialAccountApplication, 453);

        public static ServiceError ComplianceTeamCanUnlockUser => Create(ResourceConstants.ComplianceTeamCanUnlockUser, 454);

        public static ServiceError AgentUserNotFound => Create(ResourceConstants.AgentUserNotFound, 455);

        public static ServiceError CardFeeRefundStartsSince(DateTime refundSince) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.CardFeeRefundStartsSince), refundSince.ToShortDateString()), 456);

        public static ServiceError RefundExceptionDate(DateTime exceptionDate) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.RefundExceptionDate), exceptionDate.ToShortDateString()), 457);

        public static ServiceError YouCanNotViewThisUserDetails => Create(ResourceConstants.YouCanNotViewThisUserDetails, 458);

        public static ServiceError CommercialApplicationNotFound => Create(ResourceConstants.CommercialApplicationNotFound, 459);

        public static ServiceError CommercialApplicationStatusError => Create(ResourceConstants.CommercialApplicationStatusError, 460);

        public static ServiceError CommercialApplicationFileLimitsReached(int CommercialApplicationFileUploadLimits) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.CommercialApplicationFileLimitsReached), CommercialApplicationFileUploadLimits), 461);

        public static ServiceError UnableToDownloadFileFromFtp => Create(ResourceConstants.UnableToDownloadFileFromFtp, 462);

        public static ServiceError UnableToMoveFtpFile => Create(ResourceConstants.UnableToMoveFtpFile, 463);

        public static ServiceError UnableToWriteTextFtpFile => Create(ResourceConstants.UnableToWriteTextFtpFile, 464);

        public static ServiceError FormattedAddresIsTooLong(int characterCount) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.FormattedAddresIsTooLong), characterCount), 465);

        public static ServiceError MinIbanTransferLimit(decimal limit, string currencyCode)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.MinIbanTransferLimit), limit.ToMoneyString(rounded: true), currencyCode), 466);
        } // 466

        /// <summary>
        /// This error code is used by mobile teams so please don't change or overwrite.
        /// </summary>
        public static ServiceError IbanCorporateAccount => Create(ResourceConstants.IbanCorporateAccount, 467);

        public static ServiceError IbanNotTransferBank => Create(ResourceConstants.IbanNotTransferBank, 468);

        public static ServiceError QuickContactAlreayPinned => Create(ResourceConstants.QuickContactAlreayPinned, 469);

        public static ServiceError QuickContactAlreayAdded => Create(ResourceConstants.QuickContactAlreayAdded, 470);

        public static ServiceError QuickContactNotFound => Create(ResourceConstants.QuickContactNotFound, 471);

        public static ServiceError MerchantBalanceNotExists(Currency currency) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.MerchantBalanceNotExists), currency.ToString()), 472);

        public static ServiceError IbanNotFound => Create(ResourceConstants.IbanNotFound, 473);

        public static ServiceError GestureNotFound => Create(ResourceConstants.GestureNotFound, 474);

        public static ServiceError LedgerGestureRelationshipNotFound => Create(ResourceConstants.LedgerGestureRelationshipNotFound, 475);

        public static ServiceError LedgerGestureRelationshipAlreadyExist => Create(ResourceConstants.LedgerGestureRelationshipAlreadyExist, 476);

        public static ServiceError CorporatePaparaCardTransactionNotAllowed => Create(ResourceConstants.CorporatePaparaCardTransactionNotAllowed, 477);

        public static ServiceError BankNotMultiCurrencyMoneyTransferAllowed => Create(ResourceConstants.BankNotMultiCurrencyMoneyTransferAllowed, 478);

        public static ServiceError BankNotAllowedForIbanTransfer => Create(ResourceConstants.BankNotAllowedForIbanTransfer, 479);

        public static ServiceError InvitationToSameNumberExceeded(string phoneNumber)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.InvitationToSameNumberExceeded), phoneNumber), 480);

        public static ServiceError SplitUserAlreadyInvited(string phoneNumber)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.SplitUserAlreadyInvited), phoneNumber), 481);

        public static ServiceError SplitListHasDuplicateRecord => Create(ResourceConstants.SplitListHasDuplicateRecord, 482);

        public static ServiceError BankNotAllowedTemporaryForIbanTransfer => Create(ResourceConstants.BankNotAllowedTemporaryForIbanTransfer, 483);

        public static ServiceError SendMoneySenderRestriction => Create(ResourceConstants.SendMoneySenderRestriction, 484);

        public static ServiceError TransactionAlreadyExistByReferenceNumber(PaparaCardTransactionType transactionType)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.TransactionAlreadyExistByReferenceNumber), transactionType.GetDisplayName()), 485);

        public static ServiceError TransactionIssueNotFound => Create(ResourceConstants.TransactionIssueNotFound, 486);

        public static ServiceError ClosedTransactionIssue => Create(ResourceConstants.ClosedTransactionIssue, 487);

        public static ServiceError SavedCardNotFound => Create(ResourceConstants.SavedCardNotFound, 488);

        public static ServiceError SendMoneySenderBalanceRestriction => Create(ResourceConstants.SendMoneySenderBalanceRestriction, 489);

        public static ServiceError GifIsNotValid => Create(ResourceConstants.GifIsNotValid, 490);

        public static ServiceError CardHasAllowedCategory => Create(ResourceConstants.CardHasAllowedCategory, 491);

        public static ServiceError CardHasAllowedCardAcceptor => Create(ResourceConstants.CardHasAllowedCardAcceptor, 492);

        public static ServiceError OnlyComplianceUserIsAllowed => Create(ResourceConstants.OnlyComplianceUserIsAllowed, 493);

        public static ServiceError UserBelow18TransactionLimitHasExceeded(decimal limit, TransactionRemainingLimit remainingLimit, CurrencyInfo currencyInfo)
        {
            var remainingLimitRounded = remainingLimit.RemainingLimit.ToMoneyString(rounded: true);

            var currencyString = CurrencyService.GetCurrencyDisplayCode(remainingLimit.Currency);

            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.UserBelow18TransactionLimitHasExceeded), limit, currencyInfo.PreferredDisplayCode, remainingLimitRounded, currencyString), 494);
        }

        public static ServiceError SubscriptionTransactionIsNotPermitted(string brandName)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.SubscriptionTransactionIsNotPermitted), brandName), 495);

        public static ServiceError SubscriptionNotFound => Create(ResourceConstants.SubscriptionNotFound, 496);

        public static ServiceError CardClosedToContactlessTransaction =>
            Create(ResourceConstants.CardClosedToContactlessTransaction, 497);

        public static ServiceError MagneticCardContactlessTransactionSettingCanNotBeChanged(string cardTypeName) =>
            new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.MagneticCardContactlessTransactionSettingCanNotBeChanged), cardTypeName), 498);

        public static ServiceError SuspiciousMailCannotBeAdded => Create(ResourceConstants.SuspiciousMailCannotBeAdded, 499);

        public static ServiceError TokenAlreadyUsed => Create(ResourceConstants.TokenAlreadyUsed, 500);

        public static ServiceError SplitLedgerCantBeCompletedViaCard => Create(ResourceConstants.SplitLedgerCantBeCompletedViaCard, 501);

        public static ServiceError TransactionLimitExceededForSendMoneyByCardReceiver => Create(ResourceConstants.TransactionLimitExceededForSendMoneyByCardReceiver, 502);

        public static ServiceError TransactionLimitExceededForSendMoneyByCardSender(string totalLimit, string remaining) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.TransactionLimitExceededForSendMoneyByCardSender), totalLimit, remaining), 503);

        public static ServiceError SendMoneyByCardSenderCountLimitReached(int monthlyLimit) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.SendMoneyByCardSenderCountLimitReached), monthlyLimit), 504);

        public static ServiceError SendMoneyByCardReceiverCountLimitReached => Create(ResourceConstants.SendMoneyByCardReceiverCountLimitReached, 505);

        public static ServiceError EmptyJobType => Create(ResourceConstants.EmptyJobType, 506);

        public static ServiceError UserMustBe18ForPaymentToCryptoCurrencyMerchant =>
            Create(ResourceConstants.UserMustBe18ForPaymentToCryptoCurrencyMerchant, 507);

        public static ServiceError ExceedsGamePaymentMonthlyLimit(decimal monthlyLimit) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.ExceedsGamePaymentMonthlyLimit), monthlyLimit), 508);

        public static ServiceError CryptoMerchantCannotPaymentByCard => Create(ResourceConstants.CryptoMerchantCannotPaymentByCard, 509);

        public static ServiceError SingleSendMoneyCardTransactionLimitReachedForJobType(decimal limitAmount, string jobTypeName) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.SingleSendMoneyCardTransactionLimitReachedForJobType), limitAmount, jobTypeName), 510);

        public static ServiceError InvalidInstructionType => Create(ResourceConstants.InvalidInstructionType, 511);

        public static ServiceError AmountsNotMatch => Create(ResourceConstants.AmountsNotMatch, 512);

        public static ServiceError LedgerCannotBeRevertedTwice => Create(ResourceConstants.LedgerCannotBeRevertedTwice, 513);

        public static ServiceError BankTransactionAlreadyExitsCancelled => Create(ResourceConstants.BankTransactionAlreadyExitsCancelled, 514);

        public static ServiceError AllowedCashbackCountIsReached => Create(ResourceConstants.AllowedCashbackCountIsReached, 515);

        public static ServiceError PaparaCardIsNotBelongToUser => Create(ResourceConstants.PaparaCardIsNotBelongToUser, 516);

        public static ServiceError TypeIsOnlyApplicableForPositiveTransaction(string type) =>
            new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.TypeIsOnlyApplicableForPositiveTransaction), type), 517);

        public static ServiceError SelectedAtmWithdrawalAmountExceeded => Create(ResourceConstants.SelectedAtmWithdrawalAmountExceeded, 518);

        public static ServiceError PaparaCardPosManuelEntryPositiveAmountLimitExceeded(decimal positiveAmountLimit) =>
            new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.PaparaCardManuelEntryPositiveAmountLimitExceeded), positiveAmountLimit), 519);

        public static ServiceError MaxAllowedCashbackTotalIsExceeded(string calculatedReward) =>
            new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.MaxAllowedCashbackTotalIsExceeded), calculatedReward), 520);

        public static ServiceError LedgerWithSameTypeIsCancelledBefore => Create(ResourceConstants.LedgerWithSameTypeIsCancelledBefore, 521);

        public static ServiceError GestureNotAllowed => Create(ResourceConstants.GestureNotAllowed, 522);

        public static ServiceError UserMustBeOver18ForCommercialAccount => Create(ResourceConstants.UserMustBeOver18ForCommercialAccount, 523);

        public static ServiceError LedgerWithSameAmountIsEnteredBefore => Create(ResourceConstants.LedgerWithSameAmountIsEnteredBefore, 524);

        public static ServiceError ExcelReadError => Create(ResourceConstants.ExcelReadError, 525);

        public static ServiceError SameSavingBalanceDefinitionAlreadyExists => Create(ResourceConstants.SameSavingBalanceDefinitionAlreadyExists, 526);

        public static ServiceError SavingBalanceDefinitionNotExists => new ServiceError(ResourceValue(ResourceConstants.SavingBalanceDefinitionNotExists), 527);

        public static ServiceError SavingBalanceNotExists(string currency = null) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.SavingBalanceNotExists), currency), 528);

        public static ServiceError SavingBalanceLimitExceeded => Create(ResourceConstants.SavingBalanceLimitExceeded, 529);

        public static ServiceError CurrencyIsNotSupportedToDefinitionType(string currencyCode = null, string definitionType = null)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.CurrencyIsNotSupportedToDefinitionType), currencyCode, definitionType), 530);
        }

        public static ServiceError InsufficientSavingBalance => Create(ResourceConstants.InsufficientSavingBalance, 531);

        public static ServiceError SavingAmountCannotBeZero => Create(ResourceConstants.SavingAmountCannotBeZero, 532);

        public static ServiceError MinSavingAccountTransactionLimit(decimal limit, CurrencyInfo currencyInfo) =>
            new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.MinSavingAccountTransactionLimit),
                                           CurrencyService.RoundDown(limit, currencyInfo.Precision),
                                           currencyInfo.PreferredDisplayCode), 533);
        public static ServiceError GhostVirtualCardMustHaveLimit => Create(ResourceConstants.GhostVirtualCardMustHaveLimit, 534);

        /// <summary>
        /// This error code is used by mobile teams so please don't change or overwrite.
        /// </summary>
        public static ServiceError UserBlockedMoneyRequests => Create(ResourceConstants.UserBlockedMoneyRequests, 535);

        public static ServiceError UserBlockedSplitRequest => Create(ResourceConstants.UserBlockedSplitRequest, 536);

        public static ServiceError RecurringMoneyTransferSourceIsNotSupportedForSavingAccount(string transferSource)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.RecurringMoneyTransferSourceIsNotSupportedForSavingAccount), transferSource), 537);
        }

        public static ServiceError SplitRequestsBlockedUsers(string users) =>
            new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.SplitRequestsBlockedUsers), users), 538);

        public static ServiceError ComplianceLockReadFileError => Create(ResourceConstants.ComplianceLockReadFileError, 539);

        public static ServiceError SameComplianceLockFileExists => Create(ResourceConstants.SameComplianceLockFileExists, 540);

        public static ServiceError ComplianceLockFileNotExist => Create(ResourceConstants.ComplianceLockFileNotExist, 541);

        public static ServiceError ComplianceUsersCanSaveLockFile => Create(ResourceConstants.ComplianceUsersCanSaveLockFile, 542);

        public static ServiceError RecurringMoneyTransferSourceIsNotSupportedForIban(string transferSource)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.RecurringMoneyTransferSourceIsNotSupportedForIban), transferSource), 543);
        }

        public static ServiceError PreKycOcrNotFound => Create(ResourceConstants.PreKycOcrNotFound, 544);

        public static ServiceError PreKycLivenessNotFound => Create(ResourceConstants.PreKycLivenessNotFound, 545);

        public static ServiceError PreKycInfoNotFound => Create(ResourceConstants.PreKycInfoNotFound, 546);

        public static ServiceError PreKycOcrNotVerified => Create(ResourceConstants.PreKycOcrNotVerified, 547);

        public static ServiceError PreKycOcrIdentityImageIsNotValid => Create(ResourceConstants.PreKycOcrIdentityImageIsNotValid, 548);

        public static ServiceError PreKycInfoDoesNotMatchWithOcrData(string name, string surname, string birthdate, string serialNumber, string tckn) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.PreKycInfoDoesNotMatchWithOcrData), name, surname, birthdate, serialNumber, tckn), 549);

        public static ServiceError IdentityNotSupportedAtThisAction => Create(ResourceConstants.IdentityNotSupportedAtThisAction, 550);

        public static ServiceError NFCDataIsNotValid => Create(ResourceConstants.NFCDataIsNotValid, 551);


        public static ServiceError PreKycFailCountLimitReached => Create(ResourceConstants.PreKycFailCountLimitReached, 553);

        public static ServiceError KycThresholdError => Create(ResourceConstants.KycThresholdError, 554);

        public static ServiceError PhotoCheatError => Create(ResourceConstants.KYC_PHOTOCHEAT, 555);

        public static ServiceError FaceLivenessError => Create(ResourceConstants.KYC_FACELIVENESS, 556);

        public static ServiceError FaceError => Create(ResourceConstants.KYC_FACE, 557);

        public static ServiceError HiddenPhotoError => Create(ResourceConstants.KYC_HIDDENPHOTO, 558);

        public static ServiceError SignaturePhotoError => Create(ResourceConstants.KYC_SIGNATUREPHOTO, 559);

        public static ServiceError GuillocheVerificationError => Create(ResourceConstants.KYC_GUILLOCHEVERIFICATION, 560);

        public static ServiceError RainbowVerificationError => Create(ResourceConstants.KYC_RAINBOWVERIFICATION, 561);

        public static ServiceError OcrNfcSimularityError => Create(ResourceConstants.KYC_OCRNFCSIMILARITY, 562);

        public static ServiceError MrzOcrSimularityError => Create(ResourceConstants.KYC_MRZOCRSIMILARITY, 563);

        public static ServiceError MrzOcrSimularityValidationError => Create(ResourceConstants.KYC_MRZOCRSIMILARITY_VALIDATION, 563);

        public static ServiceError HologramError => Create(ResourceConstants.KYC_HOLOGRAM, 564);

        public static ServiceError HologramFaceError => Create(ResourceConstants.KYC_HOLOGRAMFACE, 565);

        public static ServiceError BkmIdAlreadyExists => Create(ResourceConstants.BkmIdAlreadyExists, 566);

        public static ServiceError CardAcceptorNameAlreadyExists => Create(ResourceConstants.CardAcceptorNameAlreadyExists, 567);

        public static ServiceError DriverBarcodeIdMatchError => Create(ResourceConstants.KYC_DRIVERBARCODEIDMATCH, 568);

        public static ServiceError FaceLivenessAIError => Create(ResourceConstants.KYC_FACELIVENESSAI, 569);

        public static ServiceError MonthlyIbanMoneyTransferCountLimitReached(int monthlyLimit)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.MonthlyIbanMoneyTransferCountLimitReached), monthlyLimit), 570);
        }

        public static ServiceError MonthlyDistinctIbanCountLimitReached(int monthlyLimit)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.MonthlyDistinctIbanCountLimitReached), monthlyLimit), 571);
        }

        public static ServiceError KycSimilarityError => Create(ResourceConstants.KycSimilarityError, 573);

        public static ServiceError ApprovalWithSameDeviceLimitError => Create(ResourceConstants.ApprovalWithSameDeviceLimitError, 574);

        public static ServiceError HologramIdSimilarityError => Create(ResourceConstants.KYC_HOLOGRAMIDSIMILARITY, 575);

        public static ServiceError ManuelCashbackTransactionLimitExceeded(decimal manuelCashbackTransactionLimit) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.ManuelCashbackTransactionLimitExceeded), manuelCashbackTransactionLimit), 576);


        #endregion

        #region FxErrors

        public static ServiceError CurrencyIsNotSupported(string currencyCode = null)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.CurrencyIsNotValid), currencyCode), 600);
        }

        public static ServiceError FxPairIsNotSupported(string pair)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.FxPairIsNotSupported), pair), 601);
        }

        public static ServiceError OrderIsLessThanMinimum(decimal minimumTotal, string currency)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.OrderIsLessThanMinimum), minimumTotal, currency), 602);
        }

        public static ServiceError DirectionIsInvalid => Create(ResourceConstants.DirectionIsInvalid, 603);

        public static ServiceError QuantityCurrencyDoesNotMatchBuyOrSellCurrency =>
            Create(ResourceConstants.QuantityCurrencyDoesNotMatchBuyOrSellCurrency, 604);

        public static ServiceError FxPriceHasExpired => Create(ResourceConstants.FxPriceHasExpired, 605);

        public static ServiceError FxRequestAndCurrentPricesDifferenceHigh => Create(ResourceConstants.FxRequestAndCurrentPricesDifferenceHigh, 606);

        public static ServiceError UserMustBeAbove18ToCreateFXBalance(CurrencyInfo currencyInfo) =>
            new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.UserMustBeAbove18ToCreateFXBalance), currencyInfo.PreferredDisplayCode), 607);

        public static ServiceError FxSellVolumeLimitHasexceeded(CurrencyInfo currencyInfo) =>
            new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.FxSellVolumeLimitHasexceeded), currencyInfo.PreferredDisplayCode), 608);

        public static ServiceError FxExchangeDailyLimitAsDollar(string limit)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.FxExchangeDailyLimitAsDollar), limit), 609);
        }

        #endregion

        #region EduPosErrors

        public static ServiceError MerchantTerminalNotFound => Create(ResourceConstants.MerchantTerminalNotFound, 701);

        public static ServiceError MerchantTerminalIsnotActive => Create(ResourceConstants.MerchantTerminalIsnotActive, 702);

        public static ServiceError TerminalDeviceNotFound => Create(ResourceConstants.TerminalDeviceNotFound, 703);

        public static ServiceError WrongTerminalSetupSmsCode => Create(ResourceConstants.WrongTerminalSetupSmsCode, 704);

        public static ServiceError TerminalAndDeviceAlreadyLinkedEachOther => Create(ResourceConstants.TerminalAndDeviceAlreadyLinkedEachOther, 705);

        public static ServiceError SearchIsFailed => Create(ResourceConstants.SearchIsFailed, 940);

        public static ServiceError OnlyActiveTerminalsCanDisable => Create(ResourceConstants.OnlyActiveTerminalsCanDisable, 706);

        #endregion

        #region BillPaymentErrors

        public static ServiceError UserSavedBillQueryExceededLimit(int userSavedBillQueryLimit) =>
            new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.UserSavedBillQueryExceededLimit), userSavedBillQueryLimit), 700);

        public static ServiceError BillCompanyInactiveHours(string errorMessage) => new ServiceError(errorMessage, 800);

        public static ServiceError BillTypeNotFound => Create(ResourceConstants.BillTypeNotFound, 801);

        public static ServiceError BillDataChanged => Create(ResourceConstants.BillDataChanged, 802);

        public static ServiceError BillAmountMissing => Create(ResourceConstants.BillAmountMissing, 803);

        public static ServiceError BillPaymentInformationMissing => Create(ResourceConstants.BillPaymentInformationMissing, 804);

        public static ServiceError BillExternalServiceFailedToProcessRequestTryAgain => Create(ResourceConstants.BillExternalServiceFailedToProcessRequestTryAgain, 806);

        public static ServiceError BillCompanySearchLimitError => Create(ResourceConstants.BillCompanySearchLimitError, 807);

        public static ServiceError BillAlreadyPaid => Create(ResourceConstants.BillAlreadyPaid, 808);

        public static ServiceError GamePurchaseAmountInvalid => Create(ResourceConstants.GamePurchaseAmountInvalid, 809);

        public static ServiceError GameMaximumQuantityInvalid => Create(ResourceConstants.GameMaximumQuantityInvalid, 805);

        public static ServiceError BillNotFound => Create(ResourceConstants.BillNotFound, 810);

        public static ServiceError BillQueryAutomaticPaymentEnabled => Create(ResourceConstants.BillQueryAutomaticPaymentEnabled, 811);

        // Seperate message for bill company and game company
        public static ServiceError CompanyNotSupportedNow(bool isBill) => isBill ? Create(ResourceConstants.BillCompanyNotSupportedNow, 812)
            : Create(ResourceConstants.GameCompanyNotSupportedNow, 812);

        public static ServiceError BillAccountNumberFormatIsNotValid => Create(ResourceConstants.BillAccountNumberFormatIsNotValid, 813);

        public static ServiceError BillMonthlyMaxPayLimitReached(int monthlyBillPayLimit)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.MonthlyMaxPayBillLimitReached), monthlyBillPayLimit), 814);

        public static ServiceError BillQueryDailyMaxLimitReached(int dailyBillQueryLimit) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.DailyMaxQueryBillLimitReached), dailyBillQueryLimit), 815);

        public static ServiceError BillQueryMonthlyMaxLimitReached(int monthlyBillQueryLimit) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.MonthlyMaxQueryBillLimitReached), monthlyBillQueryLimit), 816);

        public static ServiceError BtcWithdrawalTransactionCountExceedLimit(int limit)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.BtcWithdrawalTransactionCountExceedLimit), limit), 817);
        }

        #endregion

        #region CryptoErrors

        public static ServiceError BtcWithdrawalAddressCountShouldBeLessThanLimit => Create(ResourceConstants.BtcWithdrawalAddressCountShouldBeLessThanLimit, 818);

        public static ServiceError BtcAddressHasBeenSavedBefore => Create(ResourceConstants.BtcAddressHasBeenSavedBefore, 819);

        public static ServiceError BtcAddressWasCreatedAsDeposit => Create(ResourceConstants.BtcAddressWasCreatedAsDeposit, 820);

        public static ServiceError YouMustEnterBtcAddress => Create(ResourceConstants.YouMustEnterBtcAddress, 821);

        public static ServiceError InvalidBtcAddress => Create(ResourceConstants.InvalidBtcAddress, 822);

        public static ServiceError CryptoDefaultError => Create(ResourceConstants.CryptoDefaultErrorMessage, 823);

        public static ServiceError CurrencyNotAllowedForThisProcess => Create(ResourceConstants.CurrencyNotAllowedForThisProccess, 824);

        public static ServiceError InvalidCryptoAddress => Create(ResourceConstants.InvalidCryptoAddress, 825);

        #endregion

        #region Istanbulkart errors
        public static ServiceError IstanbulKartLoadOneTimeLimitReached(decimal istanbulKartOneTimeLimit)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.IstanbulKartLoadOneTimeLimitReached), istanbulKartOneTimeLimit), 826);

        public static ServiceError IstanbulKartLoadMonthlyCardLimitReached(decimal istanbulKartMonthlyCardLimit)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.IstanbulKartLoadMonthlyCardLimitReached), istanbulKartMonthlyCardLimit), 827);

        public static ServiceError IstanbulKartLoadMonthlyUserLimitReached(decimal istanbulKartMonthlyUserLimit)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.IstanbulKartLoadMonthlyUserLimitReached), istanbulKartMonthlyUserLimit), 828);

        public static ServiceError IstanbulKartLoadMustBeGreaterThanMinLimit(decimal istanbulKartMinLoadLimit)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.IstanbulKartLoadMustBeGreaterThanMinLimit), istanbulKartMinLoadLimit), 829);

        public static ServiceError IstanbulKartLoadShortTimeLimit(int shortTimeDefinition, int shortTimeLimit)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.IstanbulKartLoadShortTimeLimit), shortTimeDefinition, shortTimeLimit), 830);

        public static ServiceError IstanbulKartDailyDistinctCardLimit(int limit)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.IstanbulKartDailyDistinctCardLimit), limit), 831);

        public static ServiceError IstanbulKartDailyAmountLimit(int limit)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.IstanbulKartDailyAmountLimit), limit), 832);

        public static ServiceError IstanbulKartMaxBalanceLimitReached(decimal limit, decimal currentBalance, decimal maxPossibleAmount)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.IstanbulKartMaxCardBalanceLimit), limit, currentBalance, maxPossibleAmount), 833);

        #endregion

        public static ServiceError GamePackagesMissing => Create(ResourceConstants.GamePackagesMissing, 834);

        public static ServiceError DonationAmountCanNotBeLessThanAllowedMinimum(decimal minLimit)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.DonationAmountCanNotBeLessThanAllowedMinimum), minLimit), 835);

        public static ServiceError DonationAssociationNotFound => Create(ResourceConstants.DonationAssociationNotFound, 836);

        public static ServiceError DonationReferringEmailInvalid => Create(ResourceConstants.DonationReferringEmailInvalid, 837);

        public static ServiceError ReferringDonationRecurToSameCompanyNotAllowedWithSamePaymentMethod(string companyName)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.ReferringDonationRecurToSameCompanyNotAllowedWithSamePaymentMethod), companyName), 838);

        public static ServiceError ReferringDonationRecurToSameCompanyNotAllowedWithDifferentPaymentMethod(string companyName)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.ReferringDonationRecurToSameCompanyNotAllowedWithDifferentPaymentMethod), companyName), 839);

        public static ServiceError DonationRecurToSameCompanyNotAllowedWithSamePaymentMethod(string companyName, string referringEmail)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.DonationRecurToSameCompanyNotAllowedWithSamePaymentMethod), companyName, referringEmail), 840);

        public static ServiceError DonationRecurToSameCompanyNotAllowedWithDifferentPaymentMethod(string companyName, string referringEmail)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.DonationRecurToSameCompanyNotAllowedWithDifferentPaymentMethod), companyName, referringEmail), 841);

        public static ServiceError BillPayGlobalMaxLimitForAnyBillCanNotBeExceeded(decimal limit)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.BillPayGlobalMaxLimitForAnyBillCanNotBeExceeded), limit), 842);

        /// <summary>
        /// This error code is used by mobile teams so please don't change or overwrite.
        /// </summary>
        public static ServiceError UserMoneyRequestBlocked => new ServiceError(ResourceValue(ResourceConstants.UserMoneyRequestHasBeenBlocked), 843);

        public static ServiceError UserDeviceApprovementEmailWarning => Create(ResourceConstants.UserDeviceApprovementEmailWarning, 844);

        public static ServiceError UserDeviceApprovementSmsWarning => Create(ResourceConstants.UserDeviceApprovementSmsWarning, 845);

        public static ServiceError UserDeviceApprovementTokenExpire => Create(ResourceConstants.UserDeviceApprovementTokenExpire, 846);

        public static ServiceError UserDeviceNotFound => Create(ResourceConstants.UserDeviceNotFound, 847);

        public static ServiceError DonationAmountCanNotBeGreaterThanAllowedMaximum(decimal maxLimit)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.DonationAmountCanNotBeGreaterThanAllowedMaximum), maxLimit), 848);

        public static ServiceError NotSameIdentityUsed => Create(ResourceConstants.NotSameIdentityUsed, 849);

        public static ServiceError UserMoneyRequestAlreadyBlocked => new ServiceError(ResourceValue(ResourceConstants.UserMoneyRequestAlreadyBlocked), 850);

        public static ServiceError UserMoneyRequestBlockCouldNotFound => new ServiceError(ResourceValue(ResourceConstants.UserMoneyRequestBlockCouldNotFound), 851);

        public static ServiceError PaparaFamilyAlreadyAssignedToUser => Create(ResourceConstants.PaparaFamilyAlreadyAssignedToUser, 852);

        public static ServiceError PaparaFamilyIsNotAssignedToUser => Create(ResourceConstants.PaparaFamilyIsNotAssignedToUser, 853);

        public static ServiceError CouldNotFoundJobTypes => Create(ResourceConstants.CouldNotFoundJobTypes, 854);

        public static ServiceError JobTypeAlreadyExists => Create(ResourceConstants.JobTypeAlreadyExists, 855);

        public static ServiceError JobTypeIsUsed => Create(ResourceConstants.JobTypeIsUsed, 856);

        public static ServiceError CouldNotFoundJobTypeResources => Create(ResourceConstants.CouldNotFoundJobTypeResources, 857);

        public static ServiceError NoLedgerFoundAtThisMount => Create(ResourceConstants.NoLedgerFoundAtThisMount, 858);

        public static ServiceError MoneyRequestBlockedByUsers(string users) =>
            new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.MoneyRequestBlockedByUsers), users), 859);

        public static ServiceError LedgerSearchKeywordLength => Create(ResourceConstants.LedgerSearchKeywordLength, 860);

        public static ServiceError TryToPayLaterAgain => Create(ResourceConstants.TryToPayLaterAgain, 861);

        /// <summary>
        /// This error code is used by mobile teams so please don't change or overwrite.
        /// </summary>
        public static ServiceError QrCodeMatchNotFound => Create(ResourceConstants.QrCodeMatchNotFound, 862);

        /// <summary>
        /// This error code is used by mobile teams so please don't change or overwrite.
        /// </summary>
        public static ServiceError QrCodeMatchAlreadyAssigned => Create(ResourceConstants.QrCodeMatchAlreadyAssigned, 863);

        public static ServiceError PaparaCardApplicationNotAvailableForAddressChange => Create(ResourceConstants.PaparaCardApplicationNotAvailableForAddressChange, 872);

        public static ServiceError AnUserCanInviteAPersonOnlyOnce =>
            Create(ResourceConstants.AnUserCanInviteAPersonOnlyOnce, 873);

        public static ServiceError ReceivingInvitationsFromDifferentUsersMaxCountLimitExceeded => Create(ResourceConstants.ReceivingInvitationsFromDifferentUsersMaxCountLimitExceeded, 874);

        public static ServiceError SavingBalanceCurrencyMismatchWithBalanceCurrency => Create(ResourceConstants.SavingBalanceCurrencyMismatchWithBalanceCurrency, 865);

        public static ServiceError SavingBalanceUnsupportedProcedure => Create(ResourceConstants.SavingBalanceUnsupportedProcedure, 866);

        /// <summary>
        /// This error code is used by mobile teams so please don't change or overwrite.
        /// </summary>
        public static ServiceError QrCodeNotAssigned => Create(ResourceConstants.QrCodeNotAssigned, 867);

        /// <summary>
        /// This error code is used by mobile teams so please don't change or overwrite.
        /// Mobil kullanımı : KYC Akisini sonlandiriyor acildigi sayfaya geri donuyor. (PreKycDailyRetryCountLimitReached - 868)
        /// </summary>
        public static ServiceError PreKycDailyRetryCountLimitReached => Create(ResourceConstants.PreKycDailyRetryCountLimitReached, 868);

        public static ServiceError UserMustHaveBalance(CurrencyInfo currencyInfo, string language) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.UserMustHaveBalance, language), currencyInfo.Name), 869);

        /// <summary>
        /// This error code is used by mobile teams so please don't change or overwrite.
        /// </summary>
        public static ServiceError UserSendMoneyBlocked => new ServiceError(ResourceValue(ResourceConstants.UserSendMoneyBlocked), 871);
        public static ServiceError UserSendMoneyBlockedWithName(string userFullName, string defaultLanguage) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.UserSendMoneyBlockedWithName, defaultLanguage), userFullName), 871);

        public static ServiceError ExecutionDayMustBeInRange(string period) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.ExecutionDayMustBeInRange), period), 870);

        public static ServiceError InviteByGiftCardAmountCannotBeLessThanAllowed(decimal allowedMinAmount, string currencyCode) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.InviteByGiftCardAmountCannotBeLessThanAllowed), allowedMinAmount, currencyCode), 875);

        public static ServiceError InviteByGiftCardAmountCannotBeGreaterThanAllowed(decimal allowedMaxAmount, string currencyCode) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.InviteByGiftCardAmountCannotBeGreaterThanAllowed), allowedMaxAmount, currencyCode), 876);

        public static ServiceError GiftCardAmountCannotBeGreaterThanAllowed(decimal allowedMaxAmount, string currencyCode) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.GiftCardAmountCannotBeGreaterThanAllowed), allowedMaxAmount, currencyCode), 877);

        public static ServiceError GiftCardAmountCannotBeLessThanAllowed(decimal allowedMinAmount, string currencyCode) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.GiftCardAmountCannotBeLessThanAllowed), allowedMinAmount, currencyCode), 878);

        public static ServiceError NotAllowedBrandForGiftCardTransaction(string brandName) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.NotAllowedBrandForGiftCardTransaction), brandName), 879);

        public static ServiceError YouCanNotSendGiftCardToYourself => Create(ResourceConstants.YouCanNotSendGiftCardToYourself, 880);

        public static ServiceError YouCanSendOnlyOneInviteByGiftCardToSameUser => Create(ResourceConstants.YouCanNotSendGiftCardToYourself, 881);



        public static ServiceError UserMustBeApprovedForOpenGiftCard => Create(ResourceConstants.UserMustBeApprovedForOpenGiftCard, 882);

        public static ServiceError GiftCardDeliveryDateMustBeFutureDate => Create(ResourceConstants.GiftCardDeliveryDateMustBeFutureDate, 883);

        public static ServiceError FirstNameAndLastNameMustBeFilledAtTheSameTime => Create(ResourceConstants.FirstNameAndLastNameMustBeFilledAtTheSameTime, 884);

        public static ServiceError ForgotPasswordThrottling => Create(ResourceConstants.ForgotPasswordThrottling, 885);

        public static ServiceError TfaSmsThrottling => Create(ResourceConstants.TfaSmsThrottling, 886);

        public static ServiceError SendConfirmationEmailThrottling => Create(ResourceConstants.SendConfirmationEmailThrottling, 887);

        public static ServiceError OnlyMobileMethodCalledForInvitationByMassPayment => Create(ResourceConstants.OnlyMobileMethodCalledForInvitationByMassPayment, 888);

        public static ServiceError ReceivingInvitationsFromDifferentMerchantsMaxCountLimitExceeded(int maxInvitationCount) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.ReceivingInvitationsFromDifferentMerchantsMaxCountLimitExceeded), maxInvitationCount), 889);

        public static ServiceError InvitationStatusShouldBePending => Create(ResourceConstants.InvitationStatusShouldBePending, 890);

        public static ServiceError InvalidDistrainmentInput(string fieldName) => new ServiceError(
            StringAdapter.Format(ResourceValue(ResourceConstants.InvalidDistrainmentInput), fieldName), 892);

        public static ServiceError InvalidDistrainmentStateUpdate(string currentState, string nextState) => new ServiceError(
            StringAdapter.Format(ResourceValue(ResourceConstants.InvalidDistrainmentStateUpdate), currentState, nextState), 891);

        public static ServiceError CannotGetExchangeRateHistory => Create(ResourceConstants.CannotGetExchangeRateHistory, 893);

        public static ServiceError IdentitySeriesPartInformationShouldBeValid => Create(ResourceConstants.IdentitySeriesPartInformationShouldBeValid, 894);

        public static ServiceError IdentityNumberPartInformationShouldBeValid => Create(ResourceConstants.IdentityNumberPartInformationShouldBeValid, 895);

        public static ServiceError HasParentStudentRelationshipWithUserToBeBlocked => Create(ResourceConstants.HasParentStudentRelationshipWithUserToBeBlocked, 896);

        public static ServiceError BlockedUserCannotSendGiftCard => Create(ResourceConstants.BlockedUserCannotSendGiftCard, 897);

        public static ServiceError ResourceMustBeMarkedBeforeDelete => Create(ResourceConstants.ResourceMustBeMarkedBeforeDelete, 898);

        public static ServiceError IsApprovedOrVerifiedUser => Create(ResourceConstants.IsApprovedOrVerifiedUser, 899);

        public static ServiceError MinBankCardDepositLimit(decimal limit, CurrencyInfo currencyInfo) =>
            new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.MinBankCardDepositLimit),
                                           CurrencyService.RoundDown(limit, currencyInfo.Precision),
                                           currencyInfo.PreferredDisplayCode), 900);

        public static ServiceError MaxBankCardDepositLimit(decimal limit, CurrencyInfo currencyInfo) =>
            new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.MaxBankCardDepositLimit),
                                   CurrencyService.RoundDown(limit, currencyInfo.Precision),
                                   currencyInfo.PreferredDisplayCode), 901);

        public static ServiceError LimitExceededForBankCardDeposit(string totalLimit, string remaining) =>
            new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.LimitExceededForBankCardDeposit),
                totalLimit,
                remaining), 902);

        public static ServiceError YouCanNotBlockYourself => Create(ResourceConstants.YouCanNotBlockYourself, 903);

        public static ServiceError QuickContactMaxLimitReached(int limit) =>
            new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.QuickContactMaxLimitReached), limit), 904);

        public static ServiceError MerchantOfConditionMustBeSelectedForPaymentLedger => Create(ResourceConstants.MerchantOfConditionMustBeSelectedForPaymentLedger, 905);

        public static ServiceError UserAccountHasBeenLockedForSuspiciousActions => Create(ResourceConstants.UserAccountHasBeenLockedForSuspiciousActions, 906);

        public static ServiceError GiftCardTransactionCannotBeSplitted => Create(ResourceConstants.GiftCardTransactionCannotBeSplitted, 908);

        public static ServiceError LedgerShouldNotBeCancelled => Create(ResourceConstants.LedgerShouldNotBeCancelled, 909);

        public static ServiceError LedgerCannotBeSplitted => Create(ResourceConstants.LedgerCannotBeSplitted, 910);

        public static ServiceError PartnerMerchantNotFound => Create(ResourceConstants.PartnerMerchantNotFound, 911);

        public static ServiceError BrokerMerchantNotFound => Create(ResourceConstants.BrokerMerchantNotFound, 912);

        public static ServiceError InvalidDateOfBirthForGivenIdentityNumber => Create(ResourceConstants.InvalidDateOfBirthForGivenIdentityNumber, 913);

        public static ServiceError GsmNoCouldNotVerified => Create(ResourceConstants.GsmNoCouldNotVerified, 914);

        public static ServiceError DeviceUserRegistrationLimitExceeded => Create(ResourceConstants.DeviceUserRegistrationLimitExceeded, 915);

        public static ServiceError UserLoginLimitExceededWithSameDevice => Create(ResourceConstants.UserLoginLimitExceededWithSameDevice, 916);

        public static ServiceError PaparaCardTransactionTimeExceeded => Create(ResourceConstants.PaparaCardTransactionTimeExceeded, 917);

        public static ServiceError NoAtmsFoundNearby => Create(ResourceConstants.NoAtmsFoundNearby, 918);

        public static ServiceError MccNotAvailableRefund => Create(ResourceConstants.MccNotAvailableRefund, 919);

        public static ServiceError GiphyError => Create(ResourceConstants.GiphyError, 920);

        public static ServiceError DuplicateSplitUserError => Create(ResourceConstants.DuplicateSplitUserError, 921);

        public static ServiceError DailyIbanMoneyTransferLimitExceeded => Create(ResourceConstants.DailyIbanMoneyTransferLimitExceeded, 922);

        public static ServiceError ForgetMeThrottling => Create(ResourceConstants.ForgetMeThrottling, 923);

        public static ServiceError InvalidToken => Create(ResourceConstants.InvalidToken, 924);

        public static ServiceError GiftCardPaymentVolumeLimitRemaining(decimal limit, decimal volume, string currencyCode) =>
            new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.GiftCardPaymentVolumeLimitRemaining), limit, volume, currencyCode), 925);

        public static ServiceError GiftCardPaymentCountLimitReached(int limit) =>
            new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.GiftCardPaymentCountLimitReached), limit), 926);

        public static ServiceError AppVersionIsNotSupportedForKycOperation() =>
            new ServiceError(ResourceValue(ResourceConstants.AppVersionIsNotSupportedForKycOperation), 927);

        public static ServiceError UserExceedsOpenVirtualPaparaCardLimit(int openVirtualPaparaCardCountLimit) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.UserExceedsOpenVirtualPaparaCardLimit), openVirtualPaparaCardCountLimit), 928);

        public static ServiceError MassPaymentAlreadyExists => Create(ResourceConstants.MassPaymentAlreadyExist, 929);

        public static ServiceError InvalidPin => Create(ResourceConstants.InvalidPin, 930);

        public static ServiceError PreviouslyUsedPin => Create(ResourceConstants.PreviouslyUsedPin, 931);

        public static ServiceError SequentialDigitPin => Create(ResourceConstants.SequentialDigitPin, 932);

        public static ServiceError RoleAlreadyExists => Create(ResourceConstants.RolAlreadyExists, 933);

        public static ServiceError WrongPin(int remainingPinAccessAttempts)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.WrongPin), remainingPinAccessAttempts), 934);
        }
        public static ServiceError PinAccessLockedOut => Create(ResourceConstants.PinAccessLockedOut, 935);

        public static ServiceError UsersApprovelInfoControl(int day)
        {
            var message = StringAdapter.Format(ResourceValue(ResourceConstants.UsersApprovelInfoControl), day);

            return new ServiceError(message, 936);
        }

        public static ServiceError OnlyMobileActionCalledFromWeb => Create(ResourceConstants.OnlyMobileActionCalledFromWeb, 937);

        public static ServiceError ExpiredSmsCode => Create(ResourceConstants.ExpiredSmsCode, 938);

        public static ServiceError NotificationCouldNotSend => Create(ResourceConstants.PushNotificationCouldNotSend, 939);

        public static ServiceError IbanAlreadyExist => Create(ResourceConstants.IbanAlreadyExist, 941);

        public static ServiceError TransferLimitExceeded => Create(ResourceConstants.TransferLimitExceeded, 942);

        public static ServiceError MinPocketMoneyLimit(decimal limit, string currencyCode)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.MinPocketMoneyLimit), limit.ToMoneyString(rounded: true), currencyCode), 943);
        }

        public static ServiceError CategoryNotFound => Create(ResourceConstants.CategoryNotFound, 944);

        public static ServiceError AlreadyLockedOutUser => Create(ResourceConstants.AlreadyLockedOutUser, 945);

        public static ServiceError PreKycHologramNotVerified => Create(ResourceConstants.PreKycHologramNotVerified, 946);

        public static ServiceError PreKycLivenessNotVerified => Create(ResourceConstants.PreKycLivenessNotVerified, 947);

        public static ServiceError KycContentMustBeViewed => Create(ResourceConstants.KycContentMustBeViewed, 948);

        public static ServiceError PreKycInfoIsNotInValidStatus => Create(ResourceConstants.PreKycInfoIsNotInValidStatus, 949);

        public static ServiceError AlreadyHasPreKycInfo => Create(ResourceConstants.AlreadyHasPreKycInfo, 950);

        public static ServiceError PreKycTransactionId => Create(ResourceConstants.PreKycTransactionId, 951);

        public static ServiceError PreKycNotHologramSupportedIdentity => Create(ResourceConstants.PreKycNotHologramSupportedIdentity, 952);

        public static ServiceError PreKycNotOcrSupportedIdentity => Create(ResourceConstants.PreKycNotOcrSupportedIdentity, 953);

        public static ServiceError PreKycHologramNotFound => Create(ResourceConstants.PreKycHologramNotFound, 954);

        public static ServiceError RecurringMoneyTransferSenderRestriction => Create(ResourceConstants.RecurringMoneyTransferSenderRestriction, 955);

        public static ServiceError InsufficientBalanceRecurringMoneyTransfer(string language, string otherUserFullName, decimal amount, string currencyCode) =>
    new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.InsufficientBalanceRecurringMoneyTransfer, language), otherUserFullName, amount, currencyCode), 956);

        public static ServiceError InsufficientBalanceRecurringIbanMoneyTransfer(string language, string iban, decimal amount, string currencyCode) =>
           new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.InsufficientBalanceRecurringIbanMoneyTransfer, language), iban, amount, currencyCode), 957);

        public static ServiceError SplitUserExceededMaxReceiveInvitationLimit(string phoneNumber)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.SplitUserExceededMaxReceiveInvitationLimit), phoneNumber), 958);

        public static ServiceError MissingInsuranceInformations => Create(ResourceConstants.MissingInsuranceInformations, 959);

        public static ServiceError InvalidIMEIFormat => Create(ResourceConstants.InvalidIMEIFormat, 960);

        public static ServiceError NoServiceForThisIMEI => Create(ResourceConstants.NoServiceForThisIMEI, 961);

        public static ServiceError NotVerifiedOnBtk => Create(ResourceConstants.NotVerifiedOnBtk, 962);

        public static ServiceError PhoneNumberShouldBeTurkish => Create(ResourceConstants.NotVerifiedOnBtk, 963);

        public static ServiceError ThisIMEINumberHasBeenUsed => Create(ResourceConstants.ThisIMEINumberHasBeenUsed, 964);

        public static ServiceError InValidInsurance => Create(ResourceConstants.InValidInsurance, 965);

        public static ServiceError BankCardDepositInstructionNotFound => Create(ResourceConstants.BankCardDepositInstructionNotFound, 966);

        public static ServiceError BankCardDepositCurrencyNotValid => Create(ResourceConstants.BankCardDepositCurrencyNotValid, 967);

        public static ServiceError BankCardDepositCardTypeIsNotValid => Create(ResourceConstants.BankCardDepositCardTypeIsNotValid, 968);

        public static ServiceError CardHasAlreadyBankCardDepositInstruction => Create(ResourceConstants.CardHasAlreadyBankCardDepositInstruction, 969);

        public static ServiceError IdentityPhotoForUsersAbove18Error => Create(ResourceConstants.IdentityPhotoForUsersAbove18Error, 970);

        public static ServiceError BlackListedUserTCKNIsNotValid => Create(ResourceConstants.BlackListedUserTCKNIsNotValid, 971);

        public static ServiceError BelbimUnavailable => Create(ResourceConstants.BelbimUnavailable, 972);

        public static ServiceError UserIdIsNotValid => Create(ResourceConstants.UserIdIsNotValid, 973);

        public static ServiceError ResourceCannotBeChanged => Create(ResourceConstants.ResourceCannotBeChanged, 974);

        public static ServiceError CardAcceptorNameIsInBlacklist => Create(ResourceConstants.CardAcceptorNameIsInBlacklist, 975);

        public static ServiceError KycStatusLogIsNotValidForRollback => Create(ResourceConstants.KycStatusLogIsNotValidForRollback, 976);

        public static ServiceError BankWithdrawalWithFastDisabled(decimal amount, string currencyCode)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.BankWithdrawalWithFastDisabled), amount.ToMoneyString(rounded: true), currencyCode), 977);

        public static ServiceError PaparaCardApplicationAddressBlacklistItemNotFound => Create(ResourceConstants.PaparaCardApplicationAddressBlacklistItemNotFound, 978);

        public static ServiceError BankCardDeposit3dsError => Create(ResourceConstants.BankCardDeposit3dsError, 979);

        public static ServiceError InvalidLedgerForManuelEntry => Create(ResourceConstants.InvalidLedgerForManuelEntry, 980);


        public static ServiceError SupsiciousChangeIsAlreadyInserted => Create(ResourceConstants.SupsiciousChangeIsAlreadyInserted, 981);

        public static ServiceError MerchantGiftCardListNotFound => Create(ResourceConstants.MerchantGiftCardListNotFound, 982);

        public static ServiceError DuplicateFieldOnMerchantGiftCardList(string duplicateValue, string fieldName)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.DuplicateFieldOnMerchantGiftCardList), duplicateValue, fieldName), 983);

        public static ServiceError PhoneOrAccountNumberRequiredForMerchantGiftCard => Create(ResourceConstants.PhoneOrAccountNumberRequiredForMerchantGiftCard, 984);

        public static ServiceError MerchantGiftCardIncostistentFieldsOnUser(string phoneNumber, string accountNumber)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.MerchantGiftCardIncostistentFieldsOnUser), phoneNumber, accountNumber), 985);

        public static ServiceError MerchantGiftCardSingleAmountMustBeBetweenLimits => Create(ResourceConstants.MerchantGiftCardSingleAmountMustBeBetweenLimits, 986);

        public static ServiceError UserNotFoundWithAccountNumber(long accountNumber)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.UserNotFoundWithAccountNumber), accountNumber), 987);

        public static ServiceError MerchantGiftCardFileExists => Create(ResourceConstants.MerchantGiftCardFileExists, 988);

        public static ServiceError ReferringNameMustNotBeGreaterThanTheLimitAllowed(int limit)
            => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.ReferringNameMustNotBeGreaterThanTheLimitAllowed), limit), 989);

        public static ServiceError MismatchIdentityType => Create(ResourceConstants.MismatchIdentityType, 990);

        public static ServiceError NoneOfTheUsersHad18thBirthdayToday => Create(ResourceConstants.NoneOfTheUsersHad18thBirthdayToday, 991);

        public static ServiceError IdentityNotVerifiedByFirstName => Create(ResourceConstants.IdentityNotVerifiedByFirstName, 1000);

        public static ServiceError IdentityNotVerifiedByLastName => Create(ResourceConstants.IdentityNotVerifiedByLastName, 1001);

        public static ServiceError IdentityNotVerifiedBySerialNumber => Create(ResourceConstants.IdentityNotVerifiedBySerialNumber, 1002);

        public static ServiceError VirtualCardPinCannotBeChanged => Create(ResourceConstants.VirtualCardPinCannotBeChanged, 1003);

        public static ServiceError UserDeviceNotApproved => Create(ResourceConstants.UserDeviceNotApproved, 1004);

        public static ServiceError UserNotFoundWithEmail => Create(ResourceConstants.UserNotFoundWithEmail, 1005);

        public static ServiceError UserNotFoundWithPhone => Create(ResourceConstants.UserNotFoundWithPhone, 1006);

        public static ServiceError CardAcceptorMccCodeAlreadyExists => Create(ResourceConstants.CardAcceptorMccCodeAlreadyExists, 1007);

        public static ServiceError NotificationURLRequired => Create(ResourceConstants.NotificationURLRequired, 1008);

        public static ServiceError InvalidURL => Create(ResourceConstants.InvalidURL, 1009);

        public static ServiceError DuplicatePaymentReference => Create(ResourceConstants.DuplicatePaymentReference, 1010);

        public static ServiceError RefundAmountIsNotAvailable => Create(ResourceConstants.RefundAmountIsNotAvailable, 1011);

        public static ServiceError PaymentNotAvailableToRefund => Create(ResourceConstants.PaymentNotAvailableToRefund, 1012);

        public static ServiceError BankCardCouldNotVerified => Create(ResourceConstants.BankCardCouldNotVerified, 1013);

        public static ServiceError UnableToContinueProcessing => Create(ResourceConstants.UnableToContinueProcessing, 1014);

        public static ServiceError FileContentRequired => Create(ResourceConstants.FileContentRequired, 1015);

        public static ServiceError AdminDailyManuelTransactionLimitExceeded(decimal limit, decimal total)
        {
            var message = StringAdapter.Format(ResourceValue(ResourceConstants.AdminDailyManuelTransactionLimitExceeded), limit.ToMoneyString(rounded: true), total.ToMoneyString(rounded: true));

            return new ServiceError(message, 1016);
        }

        public static ServiceError AdminMonthlyManuelTransactionLimitExceeded(decimal limit, decimal total)
        {
            var message = StringAdapter.Format(ResourceValue(ResourceConstants.AdminMonthlyManuelTransactionLimitExceeded), limit.ToMoneyString(rounded: true), total.ToMoneyString(rounded: true));

            return new ServiceError(message, 1017);
        }

        public static ServiceError DayValueIsNotDefined => Create(ResourceConstants.DayValueIsNotDefined, 1018);

        public static ServiceError MonthValueIsNotDefined => Create(ResourceConstants.MonthValueIsNotDefined, 1019);

        public static ServiceError YearValueIsNotDefined => Create(ResourceConstants.YearValueIsNotDefined, 1020);

        public static ServiceError IBANBanned => Create(ResourceConstants.IBANBanned, 1021);

        public static ServiceError UserIsAlreadyRegisteredWithPlatform(string socialPlatform) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.UserIsAlreadyRegisteredWithPlatform), socialPlatform), 1022);

        public static ServiceError UserIsAlreadyRegisteredWithAnotherMethod(string socialPlatform) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.UserIsAlreadyRegisteredWithAnotherMethod), socialPlatform), 1023);

        public static ServiceError UserIsNotExistsWithPlatformMail(string socialPlatform) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.UserIsNotExistsWithPlatformMail), socialPlatform), 1024);

        public static ServiceError UserShouldLinkSocialAccount(string socialPlatform) => new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.UserShouldLinkSocialAccount), socialPlatform), 1025);

        public static ServiceError IdentityNotVerifiedByNationality => Create(ResourceConstants.IdentityNotVerifiedByNationality, 1026);

        public static ServiceError DeviceApprovalCannotBeChanged => Create(ResourceConstants.DeviceApprovalCannotBeChanged, 1027);

        public static ServiceError UserLockedOut(double remainingMinutes)
        {
            return new ServiceError(StringAdapter.Format(ResourceValue(ResourceConstants.UserLockedOut), remainingMinutes), 1028);
        }

        public static ServiceError BannerIsNotFound => Create(ResourceConstants.BannerIsNotFound, 1029);

        public static ServiceError RegistrationFlowIsNotValid => Create(ResourceConstants.RegistrationFlowIsNotValid, 1030);

        public static ServiceError UserInfoAbuseThrottling => Create(ResourceConstants.UserInfoAbuseThrottling, 1031);

        public static ServiceError DailyLimitExceededForBankCardDeposit(decimal limit, decimal restLimit, CurrencyInfo currencyInfo) =>
           new ServiceError(string.Format(ResourceValue(ResourceConstants.DailyLimitExceededForBankCardDeposit), limit.ToMoneyString(rounded: false, decimalPlaces: 2), currencyInfo.PreferredDisplayCode, restLimit.ToMoneyString(rounded: false, decimalPlaces: 2)), 1032);

        public static ServiceError MinInviteByMoneyTransferLimit(decimal limit, CurrencyInfo currencyInfo) =>
            new ServiceError(string.Format(ResourceValue(ResourceConstants.MinInviteByMoneyTransferLimit), CurrencyService.RoundDown(limit, currencyInfo.Precision), currencyInfo.PreferredDisplayCode), 1033);

        public static ServiceError CourierDeliveryAddressCannotBeChanged => Create(ResourceConstants.CourierDeliveryAddressCannotBeChanged, 1034);

        public static ServiceError MerchantBrokerRelationReportsAreNotFound => Create(ResourceConstants.MerchantBrokerRelationReportsAreNotFound, 1035);
        
        public static ServiceError UserMustBeLawfulAge => Create(ResourceConstants.UserMustBeLawfulAge, 1036);

        public static ServiceError BlackListBankForCardDeposit(string bankName) => new ServiceError(string.Format(ResourceValue(ResourceConstants.BlackListBankForCardDeposit), bankName), 1037);

        public static ServiceError KycMaintenanceModeEnabled => Create(ResourceConstants.KycMaintenanceModeEnabled, 1038);

        public static ServiceError UserHasLedger => Create(ResourceConstants.UserHasLedger, 1039);

        public static ServiceError PublicHolidaysReadFileError => Create(ResourceConstants.PublicHolidaysReadFileError, 1040);
        
        public static ServiceError AllPublicHolidayDatesAreNotBelongToSameYear => Create(ResourceConstants.AllPublicHolidayDatesAreNotBelongToSameYear, 1041);

        public static ServiceError UserDoesntHavePreKycInfoButHasBankDeposit => Create(ResourceConstants.UserDoesntHavePreKycInfoButHasBankDeposit, 1042);

        public static ServiceError PaparaCardManuelEntryNegativeMaxLimit => Create(ResourceConstants.PaparaCardManuelEntryNegativeMaxLimit, 1045);

        public static ServiceError PaparaCardManuelEntryPositiveMaxLimit => Create(ResourceConstants.PaparaCardManuelEntryPositiveMaxLimit, 1046);

        public static ServiceError PaparaCardAtmManuelEntryNegativeAmountExceeded(decimal amount) =>
            new ServiceError(string.Format(ResourceValue(ResourceConstants.PaparaCardAtmManuelEntryNegativeAmountExceeded), amount.ToMoneyString(rounded: false, decimalPlaces: 2)), 1047);

        public static ServiceError PaparaCardKkptManuelEntryNegativeAmountExceeded(decimal amount) =>
            new ServiceError(string.Format(ResourceValue(ResourceConstants.PaparaCardKkptManuelEntryNegativeAmountExceeded), amount.ToMoneyString(rounded: false, decimalPlaces: 2)), 1048);

        public static ServiceError PaparaCardManuelEntryHasPendingAccess => Create(ResourceConstants.PaparaCardManuelEntryHasPendingAccess, 1049);
        public static ServiceError BankIbanTransferWithFastDisabled(decimal amount, string currencyCode)
   => new ServiceError(string.Format(ResourceValue(ResourceConstants.BankIbanTransferWithFastDisabled), amount.ToMoneyString(rounded: true), currencyCode), 1050);

        public static ServiceError BankTransferTypeNotFound(string transferType)
           => new ServiceError(string.Format(ResourceValue(ResourceConstants.BankTransferTypeNotFound), transferType), 1051);


        public static ServiceError QRProviderCodeIsNotValid => Create(ResourceConstants.QRProviderCodeIsNotValid, 1052);

        public static ServiceError QRAmountIsNotValid => Create(ResourceConstants.QRAmountIsNotValid, 1053);

        public static ServiceError QRDescriptionIsNotValid => Create(ResourceConstants.QRDescriptionIsNotValid, 1054);

        public static ServiceError QRCrcIsNotValid => Create(ResourceConstants.QRCrcIsNotValid, 1055);

        public static ServiceError QRCodeIsNotValid => Create(ResourceConstants.QRCodeIsNotValid, 1056);

        public static ServiceError QRVersionIsNotSupported => Create(ResourceConstants.QRVersionIsNotSupported, 1057);

        public static ServiceError QRTypeIsNotSupported => Create(ResourceConstants.QRTypeIsNotSupported, 1058);

        public static ServiceError BatmanCardMustBeLiteCard => Create(ResourceConstants.BatmanCardMustBeLiteCard, 1059);

        public static ServiceError UserMustBeRegistered => Create(ResourceConstants.UserMustBeRegistered, 1060);

        public static ServiceError UserMustBeRegisteredInKycProvider => Create(ResourceConstants.UserMustBeRegisteredInKycProvider, 1061);

        public static ServiceError InvalidPassword => new ServiceError(ResourceValue(ResourceConstants.InvalidPassword), 1067);




        #region Override Equals Operator

        // Docs: https://msdn.microsoft.com/ru-ru/library/ms173147(v=vs.80).aspx

        public override bool Equals(object obj)
        {
            // If parameter cannot be cast to ServiceError or is null return false.
            var error = obj as ServiceError;

            // Return true if the error codes match. False if the object we're comparing to is nul
            // or if it has a different code.
            return Code == error?.Code;
        }

        public bool Equals(ServiceError error)
        {
            // Return true if the error codes match. False if the object we're comparing to is nul
            // or if it has a different code.
            return Code == error?.Code;
        }

        public override int GetHashCode()
        {
            return Code;
        }

        public static bool operator ==(ServiceError a, ServiceError b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (a is null || b is null)
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static bool operator !=(ServiceError a, ServiceError b)
        {
            return !(a == b);
        }

        #endregion

    }
}