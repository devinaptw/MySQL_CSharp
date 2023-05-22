using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PremierLeague
{
    public partial class Form1 : Form
    {
        static string server = "localhost";
        static string database = "premier_league";
        static string uid = "root";
        static string password = "";
        string constring = "Server=" + server + "; database=" + database + "; uid=" + uid + "; pwd=" + password + "; Allow Zero Datetime=True;";
        string[,] n_id = new string[50, 50];
        string[,] t_id = new string[50, 50];
        int jml_tim = 0, jml_nat = 0;
        DataTable dbdataset = new DataTable();

        public Form1()
        {
            InitializeComponent();
            fill_combobox();
        }

        public void fill_combobox()
        {
            MySqlConnection con = new MySqlConnection(constring);
            MySqlCommand cmd = new MySqlCommand("SELECT team_id, team_name FROM team", con);
            try
            {
                MySqlDataAdapter sda = new MySqlDataAdapter();
                dbdataset.Clear();
                sda.SelectCommand = cmd;
                sda.Fill(dbdataset);
                comboBox1.Items.Clear();
                comboBox3.Items.Clear();
                comboBox4.Items.Clear();
                for (int i = 0; i < dbdataset.Rows.Count; i++)
                {
                    jml_tim++;
                    t_id[0, i] = dbdataset.Rows[i]["team_id"].ToString();
                    t_id[1, i] = dbdataset.Rows[i]["team_name"].ToString();
                    comboBox1.Items.Add(dbdataset.Rows[i]["team_name"].ToString());
                    comboBox3.Items.Add(dbdataset.Rows[i]["team_name"].ToString());
                    comboBox4.Items.Add(dbdataset.Rows[i]["team_name"].ToString());
                }
                sda.Update(dbdataset);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            cmd = new MySqlCommand("SELECT nationality_id, nationality_name FROM nationality", con);
            try
            {
                MySqlDataAdapter sda = new MySqlDataAdapter();
                dbdataset.Clear();
                sda.SelectCommand = cmd;
                sda.Fill(dbdataset);
                comboBox2.Items.Clear();
                for (int i = 0; i < dbdataset.Rows.Count; i++)
                {
                    jml_nat++;
                    n_id[0, i] = dbdataset.Rows[i]["nationality_id"].ToString();
                    n_id[1, i] = dbdataset.Rows[i]["nationality_name"].ToString();
                    comboBox2.Items.Add(dbdataset.Rows[i]["nationality_name"].ToString());
                }
                sda.Update(dbdataset);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBox3_SelectedValueChanged(object sender, EventArgs e)
        {
            string Team = "";
            string Team_ID = "";
            // convert team name to team_id
            if (comboBox3.SelectedItem != null)
            {
                Team = comboBox3.SelectedItem.ToString();
                for (int i = 0; i < jml_tim; i++)
                {
                    if (t_id[1, i] == Team)
                    {
                        Team_ID = t_id[0, i];
                    }
                }
            }

            MySqlConnection con = new MySqlConnection(constring);
            MySqlCommand cmd = new MySqlCommand("SELECT manager.id, manager.name, manager.birthdate, team.team_name as 'Team Name', nationality.nationality_name as 'Nationality' FROM `manager` INNER JOIN team ON manager.team_id = team.team_id INNER JOIN nationality ON manager.nationality_id = nationality.nationality_id WHERE manager.working = '0' AND manager.team_id = '" + Team_ID + "';", con);
            try
            {
                MySqlDataAdapter sda = new MySqlDataAdapter();
                dbdataset.Clear();
                sda.SelectCommand = cmd;
                sda.Fill(dbdataset);
                BindingSource bSource = new BindingSource();

                bSource.DataSource = dbdataset;
                dataGridView1.DataSource = bSource;
                dataGridView1.Columns["team_id"].Visible = false;
                dataGridView1.Columns["nationality_id"].Visible = false;
                dataGridView1.Columns["team_name"].Visible = false;
                dataGridView1.Columns["nationality_name"].Visible = false;
                dataGridView1.Columns["birthdate"].ReadOnly = true;
                dataGridView1.Columns["id"].ReadOnly = true;
                sda.Update(dbdataset);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBox4_SelectedValueChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string id = dataGridView1.Rows[0].Cells["id"].Value.ToString();
            string nama = dataGridView1.Rows[0].Cells["name"].Value.ToString();
            string team = dataGridView1.Rows[0].Cells["Team Name"].Value.ToString();
            string nationality = dataGridView1.Rows[0].Cells["Nationality"].Value.ToString();
            string textTgl = dataGridView1.Rows[0].Cells["birthdate"].Value.ToString().Replace('/','-');

            string dd = textTgl[0].ToString() + textTgl[1].ToString();
            string mm = textTgl[3].ToString() + textTgl[4].ToString();
            string yy = textTgl[6].ToString() + textTgl[7].ToString() + textTgl[8].ToString() + textTgl[9].ToString();
            string birthdate = yy + '-' + mm + '-' + dd;
            string Team_ID = "";
            // convert team name to team_id
            for (int i = 0; i < jml_tim; i++) {
                if (t_id[1, i] == team)
                {
                    Team_ID = t_id[0, i];
                }
            }
            string Nationality_ID = "";
            // convert team name to team_id
            for (int i = 0; i < jml_nat; i++)
            {
                if (n_id[1, i] == nationality)
                {
                    Nationality_ID = n_id[0, i];
                }
            }
            MessageBox.Show(birthdate);
            if ((Team_ID == "")||(Nationality_ID == ""))
            {
                MessageBox.Show("Invalid Team or Nationality");
            }
            else
            {
                using (MySqlConnection con = new MySqlConnection(constring))
                {
                    try
                    {
                        con.Open();
                        string query = "UPDATE `manager` SET `name`='" + nama + "',`team_id`='" + Team_ID + "',`nationality_id`='" + Nationality_ID + "', `birthdate`='" + birthdate + "' WHERE `id`='" + id + "'";
                        using (MySqlCommand cmd = new MySqlCommand(query, con))
                        {
                            MySqlDataReader reader = cmd.ExecuteReader();
                        }
                        MessageBox.Show("Data Diedit");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            dataGridView1.Rows[e.RowIndex].ErrorText = "Concisely describe the error and how to fix it";
            e.Cancel = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string Name = textName.Text;
            string Position = textPosition.Text;
            string Height = textHeight.Text;
            string Weight = textWeight.Text;
            string Number = textNumber.Text;
            string Birthdate = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            string Nationality = "";
            string Nationality_ID = "";
            string Team = "";
            string Team_ID = "";

            // convert team name to team_id
            if (comboBox1.SelectedItem != null)
            {
                Team = comboBox1.SelectedItem.ToString();
                for(int i=0; i < jml_tim; i++)
                {
                    if (t_id[1,i] == Team)
                    {
                        Team_ID = t_id[0,i];
                    }
                }
            }
            // convert nationality to nationality_id
            if (comboBox2.SelectedItem != null)
            {
                Nationality = comboBox2.SelectedItem.ToString();
                for (int i = 0; i < jml_nat; i++)
                {
                    if (n_id[1,i] == Nationality)
                    {
                        Nationality_ID = t_id[0,i];
                    }
                }
            }

            if ((Name == "") || (Position == "") || (Height == "") || (Number == "") || (Weight == "")|| (Nationality == "")|| (Team == ""))
            {
                string message = "Please fill all fields!";
                MessageBox.Show(message);
            } 
            else
            {
                using (MySqlConnection con = new MySqlConnection(constring))
                {
                    try
                    {
                        con.Open();
                        string query =
                            "INSERT INTO `player_data`(`name`, `position`, `playing_position`, `yellow_card`,`red_card`,`goal_score`,`penalty_missed`,`height`,`weight`,`birthdate`,`number`,`status`,`team_id`,`nationality_id`) " +
                            "VALUES ('" + Name + "','DM','" + Position + "','0','0','0','0','" + Height + "','" + Weight + "','" + Birthdate + "','" + Number + "','0','" + Team_ID + "','" + Nationality_ID + "')";
                        using (MySqlCommand cmd = new MySqlCommand(query, con))
                        {
                            MySqlDataReader reader = cmd.ExecuteReader();
                        }
                        MessageBox.Show("Data Ditambahkan");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }
    }
}
