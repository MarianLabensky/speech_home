using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using MqttLib;
using System.Globalization;

namespace Speech_home
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            _client = MqttClientFactory.CreateClient("tcp://postman.cloudmqtt.com:17127", "comp1", "laivxdgr", "RQJhNBmGjTN4");
            _client.Connected += new ConnectionDelegate(client_Connected);
            _client.ConnectionLost += new ConnectionDelegate(_client_ConnectionLost);
            _client.PublishArrived += new PublishArrivedDelegate(Client_PublishArrived);

            // Setup some useful client delegate callbacks
            
            Start();

        }


        Choices numbers = new Choices();
        static TextBox t;
        static RadioButton r5;
        public static CheckBox r1, r2, r3, r4;
        static Label l3;
        static TrackBar tr1;
        static double conf = 0.6;
        static IMqtt _client;
        string str = "";
        static int check1 = 0;
        static bool checke = false;

        public void Start()
        {
            // Connect to broker in 'CleanStart' mode
            _client.Connect(true);
              
           
        }

        public void Stop()
        {
            if (_client.IsConnected)
            {
                _client.Disconnect();
            }
        }

        void client_Connected(object sender, EventArgs e)
        {
            _client.Subscribe("/test/client2", QoS.BestEfforts);
            _client.Publish("/test/client1", "1", QoS.BestEfforts, false);
        }


        void _client_ConnectionLost(object sender, EventArgs e)
        {
            MessageBox.Show("Connection lost.You have bad internet");
        }

        public void RegisterOurSubscriptions(string path)
        {
            _client.Subscribe(path, QoS.BestEfforts);
        }

        public static void Publish(string path, string value)
        {
            _client.Publish(path, value, QoS.BestEfforts, false);
        }
        

        bool Client_PublishArrived(object sender, PublishArrivedArgs e)
        {
            if (e.Topic == "/test/client2")
            {
                str = e.Payload;
                if (str == "I`m connected")
                {
                    Action act1 = () =>
                    {
                        l3.Text = "Controller connected";
                        l3.ForeColor = Color.Green;
                        Publish("/test/led1", Form1.r1.Checked == true ? "1" : "0");
                        Publish("/test/led2", Form1.r2.Checked == true ? "1" : "0");
                        Publish("/test/led3", Form1.r3.Checked == true ? "1" : "0");
                        Publish("/test/led4", Form1.r4.Checked == true ? "1" : "0");
                    };
                    Invoke(act1);
                }

                else if (str.Length == 4)
                {
                    Action act0 = () =>
                    {
                        l3.Text = "Controller connected";
                        l3.ForeColor = Color.Green;
                        r1.Checked = str[0] == '1' ? true : false;
                        r2.Checked = str[1] == '1' ? true : false;
                        r3.Checked = str[2] == '1' ? true : false;
                        r4.Checked = str[3] == '1' ? true : false;
                    };
                    Invoke(act0);
                }

                else if (str.Length == 3)
                {
                    if ( check1 == Convert.ToInt32(str))
                    {
                        Action act3 = () =>
                        {
                            l3.Text = "Controller is connected";
                            l3.ForeColor = Color.Green;
                        };
                        Invoke(act3);
                    }
                    else
                    {
                        Action act2 = () =>
                        {
                            l3.Text = "Controller is disconnected";
                            l3.ForeColor = Color.Red;
                            MessageBox.Show("Connection lost");
                        };
                            Invoke(act2);
                    }
                    
                    
                }
                checke = true;
            }
            return true;
        }
    


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            
                Publish("/test/led1", r1.Checked == true ? "1" : "0");
            
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
               Publish("/test/led2", r2.Checked == true ? "1" : "0");
            
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Publish("/test/led3", r3.Checked == true ? "1" : "0");
            
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            Publish("/test/led4", r4.Checked == true ? "1" : "0");
            
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        static void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence > conf && r5.Checked == false)
            {
                t.AppendText(e.Result.Text + "\n");
                if (e.Result.Text == "one")
                {
                    r1.Checked = r1.Checked == true ? false : true;
                    check();
                }
                if (e.Result.Text == "two")
                {
                    r2.Checked = r2.Checked == true ? false : true;
                    check();
                }
                if (e.Result.Text == "three")
                {
                    r3.Checked = r3.Checked == true ? false : true;
                    check();
                }
                if (e.Result.Text == "four")
                {
                    r4.Checked = r4.Checked == true ? false : true;
                    check();
                }

                if (e.Result.Text == "light all the lights")
                {
                    
                    r3.Checked = true;
                    r4.Checked = true;
                    r1.Checked = true;
                    r2.Checked = true;
                    check();
                }
                if (e.Result.Text == "turn off all lights")
                {
                    r1.Checked = false;
                    r2.Checked = false;
                    r3.Checked = false;
                    r4.Checked = false;
                    check();
                }
                if (e.Result.Text == "test mode")
                {
                    r5.Visible = true;
                    r5.Checked = true;
                }

            }
            else if(e.Result.Confidence > conf)
            {
                t.AppendText(e.Result.Text + "\n");
                if (e.Result.Text == "test mode")
                {
                    r5.Visible = false;
                    r5.Checked = false;
                }
            }
        }

       

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }


        static void check()
        {
            if (checke == true)
            {
                check1 = new Random().Next(100, 999);
                Publish("/test/client1", '0' + check1.ToString());
                checke = false;
            }
            else {
                MessageBox.Show("Connection lost.\nTwo last commands weren`t done!!!");
                l3.Text = "Controller is disconnected";
                l3.ForeColor = Color.Red;
            }
        }

      

        private void Form1_Shown(object sender, EventArgs e)
        {
            t = textBox1;
            r1 = checkBox1;
            r2 = checkBox2;
            r3 = checkBox3;
            r4 = checkBox4;
            tr1 = trackBar1;
            r5 = radioButton1;
            l3 = label3;
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
            SpeechRecognitionEngine sre = new SpeechRecognitionEngine(ci);
            sre.SetInputToDefaultAudioDevice();

            sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sre_SpeechRecognized);

            
            numbers.Add(new string[] { "light all the lights", "turn off all lights", "one", "two", "three", "four", "test mode"  });


            GrammarBuilder gb = new GrammarBuilder();
            gb.Culture = ci;
            gb.Append(numbers);


            Grammar g = new Grammar(gb);
            sre.LoadGrammar(g);
            sre.RecognizeAsync(RecognizeMode.Multiple);
        }
    }

 
            // Instantiate client using MqttClientFactor

         
    
}

