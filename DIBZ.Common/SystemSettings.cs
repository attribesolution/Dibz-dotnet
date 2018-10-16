using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common
{
    public static class SystemSettings
    {
        public static bool EnableSSL
        {
            get
            {
                return ConversionHelper.SafeConvertToBool(GetSettingValue(), true);
            }
        }
        public static int YearOfBirthLimit
        {
            get
            {
                return ConversionHelper.SafeConvertToInt32(GetSettingValue(),18);
            }
        }
        public static decimal SwapCharges
        {
            get
            {
                return ConversionHelper.SafeConvertToDecimal(ConfigurationManager.AppSettings["SwapCharges"], 3.00M);
            }
        }
        public static string Currency
        {
            get
            {
                return ConfigurationManager.AppSettings["Currency"];
            }
        }

        public static string PaypalClientId
        {
            get
            {
                return ConfigurationManager.AppSettings["PaypalClientId"];
            }

        }
        public static string PaymentEnvironment
        {
            get
            {
                return ConfigurationManager.AppSettings["PaymentEnvironment"];
            }
        }
        public static double PaymentTimeInHours
        {
            get
            {
                return ConversionHelper.SafeConvertToDouble(GetSettingValue(), 1);
            }   
        }
        public static int DayRule
        {
            get
            {
                return ConversionHelper.SafeConvertToInt32(GetSettingValue(), 5);
            }
        }
        public static int DayRuleStartTime
        {
            get
            {
                return ConversionHelper.SafeConvertToInt32(GetSettingValue(), 9);
            }
        }
        public static string UrlContactUsPage
        {
            get
            {
                return GetSettingValue();
            }
        }
        public static string UrlOfferDetailPage
        {
            get
            {
                return GetSettingValue();
            }
        }
        public static string UrlPossibleSwapPage
        {
            get
            {
                return GetSettingValue();
            }
        }
        // for service
        public static int SrvcPeriodicEmailHour
        {
            get
            {
                return ConversionHelper.SafeConvertToInt32(GetSettingValue(), 24);
            }
        }

        public static int SrvcDayRuleRunningTimeInMS
        {
            get
            {
                return ConversionHelper.SafeConvertToInt32(GetSettingValue(), 86400000);
            }
        }

        public static double SrvcPaymentTimeInHours
        {
            get
            {
                return ConversionHelper.SafeConvertToDouble(GetSettingValue(), 1);
            }
        }
        public static int SrvcDayRule
        {
            get
            {
                return ConversionHelper.SafeConvertToInt32(GetSettingValue(), 5);
            }
        }

        public static int SrvcDayRuleStartTime
        {
            get
            {
                return ConversionHelper.SafeConvertToInt32(GetSettingValue(), 9);
            }
        }

        #region GetSettingValue method overloads.
        private static string GetSettingValue()
        {
            try
            {
                //LogHelper.LogInfo("getting name: "+new StackFrame(1).GetMethod().Name);
                var propertyName = new StackFrame(1).GetMethod().Name.Split('_')[1];

                return ConfigurationManager.AppSettings[propertyName];
            }
            catch (Exception ex)
            {

                LogHelper.LogError(ex.Message, ex);
                return null;
            }
            
            
        }

        private static string GetSettingValue(string defaultValue)
        {
            var propertyName = new StackFrame(1).GetMethod().Name.Split('_')[1];
            return GetSettingValue(propertyName, defaultValue);
        }

        private static string GetSettingValue(string propertyName, string defaultValue)
        {
            return ConfigurationManager.AppSettings[propertyName] ?? defaultValue;
        }
        #endregion
    }
}
