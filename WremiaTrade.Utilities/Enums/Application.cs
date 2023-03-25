namespace WremiaTrade.Utilities.Enums;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Application Enums
/// </summary>
public enum Application
{
    [Display(Name = "Trade API")]
    TradeApi = 0,
    [Display(Name = "Hangfire")]
    Hangfire = 1,
    [Display(Name = "Hangfire Trigger")]
    HangfireTrigger = 2,
    [Display(Name = "Hangfire Strategy")]
    HangfireStrategy = 3,
    [Display(Name = "Hangfire Notification")]
    HangfireNotification = 4,
    [Display(Name = "Hangfire Indicator")]
    HangfireIndicator = 5,
    [Display(Name = "Hangfire Calculator")]
    HangfireCalculator = 6
}