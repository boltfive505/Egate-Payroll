using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Egate_Payroll.Objects
{
    [XmlRoot("PayslipItems")]
    public class PayslipCollectionForXml
    {
        [XmlRoot("Payslip")]
        public class PayslipItem
        {
            public string EmployeeName { get; set; }

            public string EmployeeNotes { get; set; }

            [XmlIgnore]
            public DateTime PayPeriodStart { get; set; }

            [XmlIgnore]
            public DateTime PayPeriodEnd { get; set; }

            [XmlIgnore]
            public DateTime PayDate { get; set; }

            public List<Earning> Earnings { get; set; } = new List<Earning>();

            public List<Deduction> Deductions { get; set; } = new List<Deduction>();

            [XmlIgnore]
            public decimal GrossPay { get; set; }

            [XmlIgnore]
            public decimal TotalDeductions { get; set; }

            [XmlIgnore]
            public decimal NetPay { get; set; }

            #region Xml Strings
            [XmlElement("PayPeriodStart")]
            public string _String_PayPeriodStart
            {
                get { return PayPeriodStart.ToString("yyyy-MM-dd"); }
                set { }
            }

            [XmlElement("PayPeriodEnd")]
            public string __String_PayPeriodEnd
            {
                get { return PayPeriodEnd.ToString("yyyy-MM-dd"); }
                set { }
            }

            [XmlElement("PayDate")]
            public string __String_PayDate
            {
                get { return PayDate.ToString("yyyy-MM-dd"); }
                set { }
            }

            [XmlElement("GrossPay")]
            public string __String_GrossPay
            {
                get { return GrossPay.ToString("N2"); }
                set { }
            }

            [XmlElement("TotalDeductions")]
            public string __String_TotalDeductions
            {
                get { return TotalDeductions.ToString("N2"); }
                set { }
            }

            [XmlElement("NetPay")]
            public string __String_NetPay
            {
                get { return NetPay.ToString("N2"); }
                set { }
            }
            #endregion
        }

        public class Earning
        {
            [XmlAttribute]
            public string Name { get; set; }

            [XmlIgnore]
            public TimeSpan Hours { get; set; }

            [XmlIgnore]
            public decimal Rate { get; set; }

            [XmlIgnore]
            public decimal Current { get; set; }

            #region Xml Strings
            [XmlElement("Hours")]
            public string __String_Hours
            {
                get { return string.Format("{0:00}:{1:00}", (int)Hours.TotalHours, Hours.Minutes); }
                set { }
            }

            [XmlElement("Rate")]
            public string __String_Rate
            {
                get { return Rate.ToString("N2"); }
                set { }
            }

            [XmlElement("Current")]
            public string __String_Current
            {
                get { return Current.ToString("N2"); }
                set { }
            }
            #endregion
        }

        public class Deduction
        {
            [XmlAttribute]
            public string Name { get; set; }

            [XmlIgnore]
            public decimal Current { get; set; }

            #region Xml String
            [XmlText]
            public string __String_Current
            {
                get { return Current.ToString("N2"); }
                set { }
            }
            #endregion
        }

        [XmlElement("Payslip")]
        public List<PayslipItem> PayslipItems { get; set; } = new List<PayslipItem>();

        public string ToXml()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PayslipCollectionForXml));
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.Serialize(ms, this);
                string xml = Encoding.UTF8.GetString(ms.ToArray());
                return xml;
            }
        }
    }
}
