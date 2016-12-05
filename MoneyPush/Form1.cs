using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace MoneyPush
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            listView1.View = View.Details;
            listView1.Columns.Add("Order ID", -2, HorizontalAlignment.Left);
            listView1.Columns.Add("Quantity", -2, HorizontalAlignment.Left);
            listView1.Columns.Add("Price", -2, HorizontalAlignment.Left);

            listView2.View = View.Details;
            listView2.Columns.Add("Order ID", -2, HorizontalAlignment.Left);
            listView2.Columns.Add("Quantity", -2, HorizontalAlignment.Left);
            listView2.Columns.Add("Price", -2, HorizontalAlignment.Left);

        }

        private void button1_Click(object sender, EventArgs e)
        {        
            if (String.IsNullOrEmpty(comboBox1.Text) || String.IsNullOrEmpty(textBox3.Text) || String.IsNullOrEmpty(comboBox2.Text))
            {
                MessageBox.Show("Il faut remplir les champs obligatoires");
            }

            Console.WriteLine("clicl");
            var client = new HttpClient();
            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("user_key", "4f6158ef97874d8d49ead880fc6fe756"),
                new KeyValuePair<string, string>("Symbol", comboBox1.Text),
                new KeyValuePair<string, string>("OrderQty", textBox3.Text),
                new KeyValuePair<string, string>("Side", comboBox2.Text),
            };

            var content = new FormUrlEncodedContent(pairs);

            var response = client.PostAsync("https://api-2445581154346.apicast.io/positions", content).Result;

            var contents = response.Content.ReadAsStringAsync();

            Console.WriteLine(response);
            if (response.IsSuccessStatusCode)
            {
                //Console.WriteLine(response);
                Task<string> content_Post = response.Content.ReadAsStringAsync();
                string body = content_Post.Result;
                Console.WriteLine(body);
                dynamic data = JObject.Parse(body);
                Console.WriteLine(data.Price);
                label11.Text = data.Price;
                label9.Text = "Serveur OK";
                label9.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                int Statutcode = (int)response.StatusCode;
                label9.Text = "Erreur Serveur:  "+ Statutcode.ToString();
                label9.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            listView2.Items.Clear();

            using (var client = new HttpClient())
            {
                var responseString = client.GetAsync("https://api-2445581154346.apicast.io/positions?user_key=4f6158ef97874d8d49ead880fc6fe756").Result;

                if (responseString.IsSuccessStatusCode)
                {
                    label9.Text = "Serveur OK";
                    label9.ForeColor = System.Drawing.Color.Green;
                    Task<string> content = responseString.Content.ReadAsStringAsync();
                    //string body = content.Result;

                    var objects = JArray.Parse(content.Result); // parse as array  
                    foreach (JObject root in objects)
                    {
                        foreach (KeyValuePair<String, JToken> app in root)
                        {
                            //var Order = app.Key;
                            var OrderID = (String)app.Value["OrderID"];
                            var Price = (String)app.Value["Price"];
                            var Side = (String)app.Value["Side"];
                            var OrderQty = (String)app.Value["OrderQty"];

                            if (Side == "SELL")
                            {

                                var item = new ListViewItem(new[] { OrderID, OrderQty, Price });
                                listView1.Items.Add(item);

                            }
                            else if (Side == "BUY")
                            {
                                var item = new ListViewItem(new[] { OrderID, OrderQty, Price });
                                listView2.Items.Add(item);
                            }
                        }
                    }
                }
                else {
                    int Statutcode = (int)responseString.StatusCode;
                    label9.Text = "Erreur Serveur:  " + Statutcode.ToString();
                    label9.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListViewItem item = listView1.SelectedItems[0];
            Console.WriteLine(item);
        }
    }
}
