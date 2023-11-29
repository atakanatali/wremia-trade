namespace WremiaTrade.Utilities;

public class Constants
{
     public class ConnectionStrings
     {
            public class SqlServer
            {
                public const string PaparaConnection = "PaparaConnStr";
                public const string PaparaReadOnlyConnection = "PaparaConnStrReadonly";
                public const string PaparaReportConnection = "PaparaConnStrReport";
            }

            public const string Redis = "RedisConnStr";

            public const string HangfireRedis = "HangfireRedisConnStr";

            public const string ResourceRedis = "ResourceRedisConnStr";

            public const string SettingsRedis = "SettingsRedisConnStr";

            public const string SessionTokenRedis = "SessionTokenRedisConnStr";

            public const string BankRedis = "BankRedisConnStr";

            public const string MaskedPanRedis = "MaskedPanRedisConnStr";

            public const string FlashMessageRedis = "FlashMessageRedisConnStr";

            public const string EduTerminalDeviceRedis = "EduTerminalDeviceRedisConnStr";

            public const string KycThresholdRedis = "KycThresholdRedisConnStr";

            public const string RecaptchaRedis = "RecaptchaRedisConnStr";

            public const string PaymentsHfRedis = "PaymentsHfRedisConnStr";

            public const string LoginRedis = "LoginRedisConnStr";

            public const string NotificationHfRedis = "NotificationHfRedisConnStr";

            public const string ContactsRedis = "ContactsRedisConnStr";

            public const string ExternalCardAcceptorsRedis = "ExternalCardAcceptorsRedisConnStr";

            public const string UserTemporaryRoleRedis = "UserTemporaryRoleRedisConnStr";

            public const string Mongo = "MongoConnStr";

            public const string ReportHf = "ReportHfRedisConnStr";
        }
     
     public class CronDefinitions
     {
         public const string RunEveryMinute = "* * * * *";

         public const string RunEveryOddMinutes = "1-59/2 * * * *";

         public const string RunEveryEvenMinutes = "*/2 * * * *";

         public const string RunEvery5Minutes = "*/5 * * * *";

         public const string RunEveryHour = "0 * * * *";

         public const string RunEveryHourAdd4Minute = "4 * * * *";

         public const string RunEveryHourAdd5Minute = "5 * * * *";

         public const string RunEveryHourAdd6Minute = "6 * * * *";

         public const string RunEveryHalfHour = "*/30 * * * *";

         public const string RunEveryQuarterHour = "*/15 * * * *";

         public const string RunEveryQuarterHourAdd1Minute = "1-59/15 * * * *";

         public const string RunEveryQuarterHourAdd2Minute = "2-59/15 * * * *";

         public const string RunEveryQuarterHourAdd3Minute = "3-59/15 * * * *";

         public const string RunTwiceADay = "0 0,12 * * *";

         public const string FirstDayOfEveryMonth = "00 00 1 * *";

         public const string RunEveryDayAt5Am = "0 5 * * *";

         public const string RunEveryDayAt4Am = "0 4 * * *";

         public const string RunEveryEightHour = "0 */8 * * *";

         public const string RunEveryDayAt8Am = "00 08 * * *";

         public const string RunEveryDayAtHalfPast8Am = "30 8 * * *";

         public const string RunEveryDayAt9Am = "00 09 * * *";

         public const string RunEveryDayAt9Pm = "00 21 * * *";

         public const string RunEveryDayAt6Pm = "00 18 * * *";

         public const string RunEveryWeekDayAt4Pm = "00 16 * * 1-5";

         public const string RunEveryDayAtMidnight = "0 0 * * *";

         public const string RunEveryYearFifteenToThirtyOfDecemberAtMidnight = "0 0 15-30 12 *";

     }
     
     public class Redis
     {
            /// <summary>
            /// Used to specify prefix for redis cluster
            /// </summary>
            public class Prefix
            {
                /// <summary>
                /// Access Token ları tuttuğumuz veritabanı
                /// </summary>
                public const string SessionToken = "SessionToken";

                /// <summary>
                /// Banka servislerinin hatalarının tutulduğu veritabanı
                /// </summary>
                public const string Bank = "Bank";

                /// <summary>
                /// Hangfire Debug joblarının tutulduğu veritabanı
                /// </summary>
                public const string Hangfire = "{hangfire:Hangfire}:";

                /// <summary>
                /// MaskedPanların tutulduğu veritabanı
                /// </summary>
                public const string MaskedPan = "MaskedPan";

                /// <summary>
                /// App/web.config dosyasından redis dbindex'in okunması için kullanılan index.
                /// </summary>
                public const string Settings = "Settings";

                /// <summary>
                /// General Key
                /// Currently holds data for below logics/entities
                /// LedgerSummary
                /// KkbAccessToken
                /// ViewUserDetail For Admin rights
                /// RequestThrottling
                /// SanctionReportIndex
                /// SmsRecords -- Last sms sent to primary or not
                /// GetUserAuditLog For Admin rights
                /// KKB paket limitlerinin tutulduğu db
                /// </summary>
                public const string Default = "Default";
                
                /// <summary>
                /// Redis DbIndex to hold the recaptcha token for 1 minute
                /// To auto accept human verification from web
                /// </summary>
                public const string Recaptcha = "Recaptcha";

                /// <summary>
                /// Used by Payments hangfire.
                /// </summary>
                public const string PaymentsHf = "{hangfire:PaymentsHf}:";
                
                /// <summary>
                /// Used by Notifications hangfire.
                /// </summary>
                public const string NotificationsHf = "{hangfire:NotificationsHf}:";

                /// <summary>
                /// Used by report hangfire.
                /// </summary>
                public const string ReportHf = "{hangfire:ReportHf}:";

                /// <summary>
                /// resource
                /// </summary>
                public const string Resource = "Resource";
            }
            
            public const string ReportRedisPrefix = "ReportRedisPrefix";

            /// <summary>
            /// Monitoring:{key}
            /// Monitoring:heartbeat
            /// Monitoring işlemleri için kullanılacak keylere ait prefix
            /// </summary>
            public const string MonitoringRedisPrefix = "Monitoring";

            /// <summary>
            /// Bu key'i dakikada bir ekle / oku / sil yaparak redis' e erişilip erişelemediğini monitor edeceğiz.
            /// </summary>
            public const string MonitoringHeartBeat = MonitoringRedisPrefix + ":HeartBeat";

            /// <summary>
            /// Request throttling redis path.
            /// Ex: Operation:Throttling:UrlPath:[controller_action]:[userId_durationType]
            /// </summary>
            public const string RequestThrottling = "Operation:Throttling:UrlPath:{0}:{1}";

            public const string SmsRecords = "SmsRecords:{0}";

            public const string SmsProviderSettings = "Operation:Setting:SmsProviders";

            /// <summary>
            /// SessionToken_{Guid}
            /// </summary>
            public const string SessionToken = "Session_{0}";
        }
     
     public class System
     {
            /// <summary>
            /// Redis UI
            /// </summary>
            public const string RedisInsightWebAddress = "";
            
            /// <summary>
            /// Elasticvute
            /// </summary>
            public const string ElasticInsightWebAddress = "";
            
            /// <summary>
            /// Kibana Elastic UI
            /// </summary>
            public const string KibanaWebAddress = "";
            
            /// <summary>
            /// Trade Api Base URL
            /// </summary>
            public const string ApiTradeWebAddress = "";
            
            /// <summary>
            /// ES01 Elastic host
            /// </summary>
            public const string Es01WebAddress = "";
            
            /// <summary>
            /// Mongo01 database host
            /// </summary>
            public const string Mongo01WebAddress = "";
            
            /// <summary>
            /// Calculator Hangfire Host
            /// </summary>
            public const string CalculatorHangfireWebAddress = "";
            
            /// <summary>
            /// Indicator Hangfire Host
            /// </summary>
            public const string IndicatorHangfireWebAddress = "";
            
            /// <summary>
            /// Notification Hangfire Host
            /// </summary>
            public const string NotificationHangfireWebAddress = "";
            
            /// <summary>
            /// Strategy Hangfire Host
            /// </summary>
            public const string StrategyHangfireWebAddress = "";
            
            /// <summary>
            /// Trigger Hangfire Host
            /// </summary>
            public const string TriggerHangfireWebAddress = "";
            
            /// <summary>
            /// Hangfire Host
            /// </summary>
            public const string HangfireWebAddress = "";
            
            /// <summary>
            /// Trader Portal Host
            /// </summary>
            public const string PortalWebAddress = "";
        }
}