using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;


namespace KurBilgileri
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        DataTable dt = new DataTable();
        private void Form1_Load(object sender, EventArgs e)
        {
            dt.Columns.Add("Birim");
            dt.Columns.Add("Döviz Adı");
            dt.Columns.Add("Alış Fiyatı");
            dt.Columns.Add("Satış Fiyatı");
            
        }

        private void btnGetir_Click(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value>DateTime.Now)
            {
                //gelecek tarih için dosya zaten olmayacak
            }
            dt.Clear();
            string xmlYolu = "";
            //bugün için http://www.tcmb.gov.tr/kurlar/today.xml
            //geçmiş için http://www.tcmb.gov.tr/kurlar/201502/12022015.xml

            if (dateTimePicker1.Value.ToShortDateString()==DateTime.Now.ToShortDateString())//bugün için kur bilgisi gelsin
            {
                xmlYolu = "http://www.tcmb.gov.tr/kurlar/today.xml";
            }
            else
            {
                xmlYolu = "http://www.tcmb.gov.tr/kurlar/" + dateTimePicker1.Value.Year.ToString("0000") + dateTimePicker1.Value.Month.ToString("00") + "/" +
                    dateTimePicker1.Value.Day.ToString("00") +
                    dateTimePicker1.Value.Month.ToString("00") +
                    dateTimePicker1.Value.Year.ToString("0000") + ".xml";
            }
            XmlTextReader okuyucu = new XmlTextReader(xmlYolu);
            XmlDocument dokuman = new XmlDocument();
            try
            {
                dokuman.Load(okuyucu);
            }
            catch 
            {
                MessageBox.Show("bu tarihe ait bir kur bilgisi yok");
            }
            XmlElement rootEleman = dokuman.DocumentElement;
            XmlNodeList currencyList = dokuman.GetElementsByTagName("Currency");
            foreach (XmlNode item in currencyList)
            {
                if (item["Unit"]==null || item["Isim"]==null || item["ForexBuying"]==null || item["ForexSelling"]==null)
                {
                    continue;//eğer eksik bilgi varsa döviz bilgisini almayıver.
                }
                DataRow satir = dt.NewRow();
                satir[0] = item["Unit"].InnerText;
                satir[1] = item["Isim"].InnerText;
                satir[2] = item["ForexBuying"].InnerText;
                satir[3] = item["ForexSelling"].InnerText;

                dt.Rows.Add(satir);
            }
            lbDoviz.DataSource = dt;
            lbDoviz.DisplayMember = "Döviz Adı";
        }

        private void lbDoviz_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbDoviz.SelectedIndex>-1)
            {
                int index=lbDoviz.SelectedIndex;
                lblBirim.Text = dt.Rows[index][0].ToString();
                lblAlis.Text = dt.Rows[index][2].ToString();
                lblSatis.Text = dt.Rows[index][3].ToString();
            }
        }

        private void btnHesap_Click(object sender, EventArgs e)
        {
            int miktar;
            double neKadar = double.Parse(lblAlis.Text.Replace(".",","));
            if (int.TryParse(maskedTextBox1.Text, out miktar))
            {
                txtHesap.Text = (miktar * neKadar).ToString();
            }
        }
    }
}
