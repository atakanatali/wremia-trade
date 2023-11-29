namespace WremiaTrade.Models.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    using WremiaTrade.ConfigAdapter;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using Newtonsoft.Json;

    [Table("Users")] // Name of the table

    [Index(nameof(UserName), Name = "IX_UserName", IsUnique = true)] // UserName Index , is unique

    [Index(nameof(Email), Name = "IX_Email", IsUnique = false)] // Email Index, not unique

    [Index(nameof(PhoneNumber), Name = "IX_PhoneNumber", IsUnique = true)]

    [Index(nameof(AccountNumber), Name = "AccountNumber_Index", IsUnique = true)]

    [Index(nameof(CreatedAt), Name = "IX_CreatedAt", IsUnique = false)]

    [Index(nameof(Id), Name = "PK_dbo.Users", IsUnique = true)]

    // Üzerinde JsonPropery olmayan bütün alanları hariç tutar, json içerisinde dahil etmez.
    // Miras alının sınıftan herhangi bir özelliği de eklemek istersek
    // override edip tepesine [JsonProperty] eklememiz yeterli oluyor
    [JsonObject(MemberSerialization.OptIn)]
    public class User : IdentityUser
    {
        public User()
        {
            CreatedAt = DateTimeHelper.LocalTime();
            Ledgers = new List<LedgerEntry>();
            Balances = new List<UserBalance>();
        }

        /// <summary>
        /// Created date
        /// </summary>
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }
        
        [JsonProperty("firstName")]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        [MaxLength(100)]
        public string LastName { get; set; }

        [JsonProperty("userType")]
        public UserType UserType { get; set; }

        [MaxLength(100)]
        public override string UserName { get; set; }

        [MaxLength(100)]
        [JsonProperty("email")]
        public override string Email { get; set; }

        [MaxLength(30)]
        [JsonProperty("phoneNumber")]
        public override string PhoneNumber { get; set; }

        /// <summary>
        /// 10 digit Papara Account number.
        /// </summary>
        [JsonProperty("accountNumber")]
        public long AccountNumber { get; set; }

        /// <summary>
        /// Kullanıcının hesap bakiyeleri
        /// </summary>
        [JsonProperty("balances")]
        public ICollection<UserBalance> Balances { get; set; }

        /// <summary>
        /// Temperory nullable tckn property
        /// private set will public when all national id implementations changed to nullable
        /// Column name will be changed to TurkishNationalId when implementations finished
        /// </summary>
        [JsonProperty("turkishNationalId")]
        [Column("TurkishNationalId")]
        public long? TCKN { get; private set; }

        private long _turkishNationalId;

        /// <summary>
        /// Turkish National Id will be nullable to be able to add unique index
        /// This property will be replaced with nullable TCKN when all implementations finished
        /// </summary>
        [Obsolete]
        [NotMapped]
        public long TurkishNationalId
        {
            get => TCKN ?? 0;
            set
            {
                if (value == 0)
                {
                    TCKN = null;
                }
                else
                {
                    TCKN = value;
                }
                _turkishNationalId = value;
            }
        }

        /// <summary>
        /// İşlem geçmişi
        /// </summary>
        [JsonProperty("ledgers")]
        public ICollection<LedgerEntry> Ledgers { get; set; }

        [NotMapped]
        [JsonProperty("fullName")]
        public string FullName => ($"{FirstName} {LastName}").Trim();

        /// <summary>
        /// Optimistic concurrency için kullanılan property.
        /// </summary>
        public long Tick { get; set; }

        [MaxLength(100)]
        public override string PasswordHash { get; set; }

        [MaxLength(50)]
        public override string SecurityStamp { get; set; }

        /// <summary>
        /// Defines user's default currency
        /// TODO : This will be a mapped propery. For now our default currency is TRY for all users.
        /// </summary>
        [JsonProperty("defaultCurrency")]
        [NotMapped]
        public Currency DefaultCurrency { get; set; } = Currency.TRY;

        #region Public Methods
        /// <summary>
        /// Verilen ledger objesine göre balance'ı update eder,
        /// ledger objesinin içindeki resulting balance da update olur.
        /// LedgerEntry'yi kullanıcının ledger'ına ekler.
        /// Eğer kullanıcının verilen entry'nin para biriminde Balance'ı yoksa ekler.
        /// </summary>
        public void AddLedger(LedgerEntry ledger)
        {
            var balanceToUpdate = Balances.FirstOrDefault(b => b.Currency == ledger.Currency);

            if (balanceToUpdate == null)
            {
                balanceToUpdate = new UserBalance
                {
                    Currency = ledger.Currency,
                };

                Balances.Add(balanceToUpdate);
            }

            balanceToUpdate.UpdateTotalWithLedger(ledger);

            Ledgers.Add(ledger);

            UpdateEntityVersion();
        }
        
        /// <summary>
        /// Hesabın kilitli olup olmadığını gösterir
        /// </summary>
        public bool IsLockedOut => LockoutEnd.HasValue && LockoutEnd > DateTime.UtcNow;

        /// <summary>
        /// Kullanıcının pasif olması operasyon tarafından mı yapıldı yoksa kullanıcı şifresini bloke ederek mi pasif yapıldı.
        /// Kullanıcı şifresini bloke ettiğinide bu süre "UserDefaultLockDuration" dakika kadardır.
        /// </summary>
        public bool IsPassive
        {
            get
            {
                int.TryParse(ConfigurationAdaptor.Manage.GetValueFromAppSettings<string>("UserDefaultLockDuration"), out var defLockDuration);

                defLockDuration = defLockDuration == default ? 60 : defLockDuration;

                return IsLockedOut;
            }
        }
        
        #endregion

        /// <summary>
        /// Optimistic Concurency sağlanması için gerekli metod.
        /// 
        /// Bir Request User nesnesi üzerinde bir operasyon yaparken, diğer bir Request' inde
        /// aynı User nesnesi ile çalışabiliyor olma ihtimalini göz önünde bulundurmalıdır.
        /// Aksi halde, User nesnesi üzerindeki yeni değişikliklerden haberdar olmayarak,
        /// hatalı operasyonlar yapması mümkündür.
        /// 
        /// User nesnesinin tutarlılığını sağlamak için DB seviyesinde this.RowVersion alanı eklenmiştir.
        /// User nesnesi üzerinde bir değişiklik olunca bu değer otomatik olarak güncellenir.
        /// Ancak User nesnesinin collection property' lerinde yapılan değişiklikler this.RowVersion tarafından algılanamaz.
        /// Bu nedenle, Collection Property' lerde olan bir değişiklikten sonra manuel olarak
        /// User nesnesinin versionunu bu metod ile güncellenmelidir.
        /// 
        /// https://msdn.microsoft.com/en-us/library/aa0416cz(v=vs.110).aspx
        /// https://msdn.microsoft.com/en-us/library/jj592904(v=vs.113).aspx
        /// </summary>
        public void UpdateEntityVersion()
        {
            Tick = DateTime.Now.ToUniversalTime().Ticks;
        }

        /// <summary>
        /// Optimistic concurrency için kullanılan property.
        /// </summary>
        [Timestamp]
        public byte[] RowVersion { get; set; }

        /// <summary>
        /// Kullanıcının blokeli bakiyeleri
        /// </summary>
        [JsonIgnore]
        public ICollection<LockedBalanceTransaction> LockedBalanceTransactions { get; set; }
    }
}