using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SymboScript
{
    public partial class Form1 : Form
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();
        [System.Runtime.InteropServices.DllImport("kernel32")]
        public static extern void FreeConsole();
        [System.Runtime.InteropServices.DllImport("kernel32")]
        public static extern void AttachConsole(int processId);
        string SS_name = "", SSR_name = "";

        public Form1()
        {
            InitializeComponent();
        }
        public struct Complex
        {
            public double R, I;
        }
        public struct Quaternion
        {
            public double R, I, J, K;
        }
        public enum Err_type
        {
            DEFAULT, TYPE_ERR, UNKNOWN_VAR, DEVIDED_ZERO, SYNTEX_ERR
        }

        private void compile(object sender, EventArgs e)
        {
            String CodeBox_Text = " ";
            if (SSR_name == "")
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Title = "Folder";
                dialog.Filter = "SSR(*.ssr)|*.ssr";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SSR_name = dialog.FileName;
                }
            }
            if (SSR_name != "")
            {
                StreamReader sr = new StreamReader(SSR_name);
                CodeBox_Text = sr.ReadLine();
                sr.Close();
            }

            Err_type err_message = Err_type.DEFAULT;
            String output = "", err_output = "";
            int Abstra = 0,
                Intro = 0,
                ptr_now = 0, 
                ptr_end = 0, //句法最尾端的位置
                fun_escape = 0, //函數引用最尾端的位置
                fun_escape2 = 0, //函數引用最尾端的位置
                lib_escape = 0;
            //類型系統：I-整數，F-浮點數，S-字串 C-複數， Q-四元數
            List<String> S_name = new List<String>();
            List<String> S_value = new List<String>();
            List<String> I_name = new List<String>();
            List<Int32> I_value = new List<Int32>();
            List<String> F_name = new List<String>();
            List<Double> F_value = new List<Double>();
            List<String> C_name = new List<String>();
            List<Complex> C_value = new List<Complex>();
            List<String> Q_name = new List<String>();
            List<Quaternion> Q_value = new List<Quaternion>();
            List<String> FUN_name = new List<String>();
            List<uint> FUN_position = new List<uint>();
            List<List<String>> FUN_variant = new List<List<String>>();
            List<List<String>> FUN_value = new List<List<String>>();
            List<String> LIB_name = new List<String>();
            List<List<String>> LIB_value = new List<List<String>>();
            bool B_temp = false, B_total = false;
            String S_temp = "";
            Int32 I_temp = 0;
            Double F_temp = 0.0f;
            Complex C_temp = new Complex();
            Quaternion Q_temp = new Quaternion();
            //進入函式
            int IndexOf_name = 0, IndexOf_name2 = 0;
            bool in_tokens = true, in_function = false, in_function2 = false, in_library = false;

            string[] raw_tokens = CodeBox_Text.Split(' ', '\n');
            int raw = raw_tokens.Length;
            string[] tokens = new string[raw]; int not_null = 0;
            for (int r = 0; r < raw; r++) { if (raw_tokens[r] != "") { tokens[not_null] = raw_tokens[r]; not_null++; } }
            int code_length = tokens.Length;
            for (int r = Abstra; r < code_length; r++ )
             {
                err_message = Err_type.DEFAULT;
                string[] string_switch = null;
                if (in_function) 
                    string_switch = FUN_value[IndexOf_name].ToArray();
                else if (in_function2)
                    string_switch = FUN_value[IndexOf_name2].ToArray();
                else if (in_library)
                    string_switch = LIB_value[LIB_value.Count - 1].ToArray();
                else if (in_tokens) 
                    string_switch = tokens;

                string str_now = null;
                if (in_function)
                {
                    if (r < FUN_value[IndexOf_name].Count)
                        str_now = FUN_value[IndexOf_name][r];
                    else
                        continue;
                }
                else if (in_function2)
                {
                    if (r < FUN_value[IndexOf_name2].Count)
                        str_now = FUN_value[IndexOf_name2][r];
                    else
                        continue;
                }
                else if (in_library)
                    str_now = LIB_value[LIB_value.Count - 1][r];
                else if (in_tokens) 
                    str_now = tokens[r];
                #region 輸出到螢幕
                //格式 : string ; _|換行
                if (str_now == ":")
                {
                    ptr_now = r;

                    for (int f = ptr_now; f < code_length; f++)
                    {
                        String str_next = string_switch[f];
                        if (str_next == ";")
                        {
                            ptr_end = f; break;
                        }
                    }

                    for (int f = ptr_now + 1; f < ptr_end; f++)
                    {
                        if (string_switch[f + 1] == "@I")
                        {
                            int amount = I_name.Count;
                            if (amount>0)
                            {
                                int found = -1;
                                for (int I = 0; I < amount; I++ )
                                {
                                    if (I_name[I] == string_switch[f])
                                    {
                                        found = I; break;
                                    }
                                }
                                if (found != -1)
                                    output += I_value[found].ToString() + " ";
                                else { err_message = Err_type.UNKNOWN_VAR; }
                            }
                            else { err_message = Err_type.TYPE_ERR; }
                            f ++;//跳到@I之後
                        }
                        else if (string_switch[f + 1] == "@F")
                        {
                            int amount = F_name.Count;
                            if (amount > 0)
                            {
                                int found = -1;
                                for (int I = 0; I < amount; I++)
                                {
                                    if (F_name[I] == string_switch[f])
                                    {
                                        found = I; break;
                                    }
                                }
                                if (found != -1)
                                    output += F_value[found].ToString() + " ";
                                else { err_message = Err_type.UNKNOWN_VAR; }
                            }
                            else { err_message = Err_type.TYPE_ERR; }
                            f ++;//跳到@F之後
                        }
                        else if (string_switch[f + 1] == "@S")
                        {
                            int amount = S_name.Count;
                            if (amount > 0)
                            {
                                int found = -1;
                                for (int I = 0; I < amount; I++)
                                {
                                    if (S_name[I] == string_switch[f])
                                    {
                                        found = I; break;
                                    }
                                }
                                if (found != -1)
                                    output += S_value[found] + " ";
                                else { err_message = Err_type.UNKNOWN_VAR; }
                            }
                            else { err_message = Err_type.TYPE_ERR; }
                            f ++;//跳到@S之後
                        }
                        else if (string_switch[f + 1] == "@C")
                        {
                            int amount = C_name.Count;
                            if (amount > 0)
                            {
                                int found = -1;
                                for (int I = 0; I < amount; I++)
                                {
                                    if (C_name[I] == string_switch[f])
                                    {
                                        found = I; break;
                                    }
                                }
                                if (found != -1)
                                    output += " \\"+C_value[found].R.ToString() + "|" + C_value[found].I.ToString() + "/ ";
                                else { err_message = Err_type.UNKNOWN_VAR; }
                            }
                            else { err_message = Err_type.TYPE_ERR; }
                            f ++;//跳到@F之後
                        }
                        else if (string_switch[f + 1] == "@Q")
                        {
                            int amount = Q_name.Count;
                            if (amount > 0)
                            {
                                int found = -1;
                                for (int I = 0; I < amount; I++)
                                {
                                    if (Q_name[I] == string_switch[f])
                                    {
                                        found = I; break;
                                    }
                                }
                                if (found != -1)
                                output += "\\" + Q_value[found].R.ToString() + "|" + Q_value[found].I.ToString() + "|"
                                    + Q_value[found].J.ToString() + "|" + Q_value[found].K.ToString() + "/ ";
                                else { err_message = Err_type.UNKNOWN_VAR; }
                            }
                            else { err_message = Err_type.TYPE_ERR; }
                            f ++;//跳到@F之後
                        }
                        else if (string_switch[f] == "_|")
                        {
                            output += "\n"; //換行
                        }
                        else
                        {
                            String str_next = string_switch[f];
                            output += str_next + " ";
                        }
                    }
                    r = ptr_end;
                }
                #endregion
                #region 宣告變數
                //格式name @ type ? value !
                else if (str_now == "@")
                {
                    ptr_now = r;
                    String str_last = string_switch[ptr_now + 1];
                    if (str_last == "I")
                    {
                        if (string_switch[ptr_now + 2] == "?")
                        {
                            I_name.Add(string_switch[ptr_now - 1]);
                            I_value.Add(0);
                            r += 2;
                        }
                        if (string_switch[ptr_now + 4] == "!")
                        {
                            int pos = I_value.Count;
                            string input = string_switch[ptr_now + 3];
                            int test = 0;
                            bool result = int.TryParse(input, out test);
                            if (result)
                                I_value[pos - 1] = test;
                            else { err_message = Err_type.TYPE_ERR; }
                            r += 2;
                        }
                    }
                    else if (str_last == "F")
                    {
                        if (string_switch[ptr_now + 2] == "?")
                        {
                            F_name.Add(string_switch[ptr_now - 1]);
                            F_value.Add(0);
                            r += 2;
                        }
                        if (string_switch[ptr_now + 4] == "!")
                        {
                            int pos = F_value.Count;
                            string input = string_switch[ptr_now + 3];
                            Double test = 0;
                            bool result = Double.TryParse(input, out test);
                            if (result)
                                F_value[pos - 1] = test;
                            else { err_message = Err_type.TYPE_ERR; }
                            r += 2;
                        }
                    }
                    else if (str_last == "S")
                    {
                        if (string_switch[ptr_now + 2] == "?")
                        {
                            S_name.Add(string_switch[ptr_now - 1]);
                            S_value.Add("");
                            r += 2;
                        }
                        if (string_switch[ptr_now + 4] == "!")
                        {
                            string[] str_to_read = string_switch[ptr_now + 3].Split('*');
                            int str_leng = str_to_read.Length;
                            int pos = S_value.Count;
                            for (int str_pos = 0; str_pos < str_leng; str_pos++)
                            {
                                S_value[pos - 1] += str_to_read[str_pos] ;
                                if (str_pos != str_leng - 1)
                                    S_value[pos - 1] += " ";
                            }
                            r += 2;
                        }
                    }
                    else if (str_last == "C")
                    {
                        if (string_switch[ptr_now + 2] == "?")
                        {
                            C_name.Add(string_switch[ptr_now - 1]);
                            Complex temp = new Complex();
                            temp.R = 0.0; temp.I = 0.0; 
                            C_value.Add(temp);
                            r += 2;
                        }
                        if (string_switch[ptr_now + 5] == "!")
                        {
                            int pos = C_value.Count;
                            string input_R = string_switch[ptr_now + 3],
                                input_I = string_switch[ptr_now + 4];
                            double test_R = 0.0, test_I = 0.0;
                            Complex temp = new Complex();
                            bool result_R = double.TryParse(input_R, out test_R),
                                 result_I = double.TryParse(input_I, out test_I);
                            if (result_R && result_I)
                            {
                                temp.R = test_R; temp.I = test_I;
                                C_value[pos - 1] = temp;
                            }
                            else { err_message = Err_type.TYPE_ERR; }
                            r += 2;
                        }
                    }
                    else if (str_last == "Q")
                    {
                        if (string_switch[ptr_now + 2] == "?")
                        {
                            Q_name.Add(string_switch[ptr_now - 1]);
                            Quaternion temp = new Quaternion();
                            temp.R = 0.0; temp.I = 0.0; temp.J = 0.0; temp.K = 0.0; 
                            Q_value.Add(temp);
                            r += 2;
                        }
                        if (string_switch[ptr_now + 7] == "!")
                        {
                            int pos = Q_value.Count;
                            string input_R = string_switch[ptr_now + 3],
                                input_I = string_switch[ptr_now + 4],
                                input_J = string_switch[ptr_now + 5],
                                input_K = string_switch[ptr_now + 6];
                            double test_R = 0.0, test_I = 0.0, test_J = 0.0, test_K = 0.0;
                            Quaternion temp = new Quaternion();
                            bool result_R = double.TryParse(input_R, out test_R),
                                 result_I = double.TryParse(input_I, out test_I),
                                 result_J = double.TryParse(input_J, out test_J),
                                 result_K = double.TryParse(input_K, out test_K);
                            if (result_R && result_I && result_J && result_K)
                            {
                                temp.R = test_R; temp.I = test_I; temp.J = test_J; temp.K = test_K;
                                Q_value[pos - 1] = temp;
                            }
                            else { err_message = Err_type.TYPE_ERR; }
                            r += 2;
                        }
                    }
                }
                #endregion
                #region 運算符 
                    //加+減-乘~除_等於=
                else if (str_now == "+" || str_now == "-" || str_now == "~" || str_now == "_" || str_now == "=" || str_now == "||")
                {
                    ptr_now = r;
                    string input1 = string_switch[ptr_now - 1], input2 = string_switch[ptr_now + 1];
                    int int1 = 0, int2 = 0;
                    Double flo1 = 0.0f, flo2 = 0.0f;
                    Complex com1 = new Complex(), com2 = new Complex();
                    Quaternion qua1 = new Quaternion(), qua2 = new Quaternion();
                    bool is_int1 = int.TryParse(input1, out int1),
                        is_int2 = int.TryParse(input2, out int2),
                        is_flo1 = Double.TryParse(input1, out flo1),
                        is_flo2 = Double.TryParse(input2, out flo2),
                        is_com1 = Complex_TryParse(input1, out com1),
                        is_com2 = Complex_TryParse(input2, out com2),
                        is_qua1 = Quaternion_TryParse(input1, out qua1),
                        is_qua2 = Quaternion_TryParse(input2, out qua2);
                    if (str_now == "=")
                    {
                        String str_next_2 = string_switch[ptr_now + 2];
                        if (str_next_2 == "@I")
                        {
                            int amount = I_name.Count;
                            if (amount > 0)
                            {
                                int found = -1;
                                for (int I = 0; I < amount; I++)
                                {
                                    if (I_name[I] == string_switch[ptr_now + 1])
                                    {
                                        found = I; break;
                                    }
                                }
                                if (found != -1)
                                {
                                    if (string_switch[ptr_now + 3] == "+")
                                        I_value[found] += I_temp;
                                    else if (string_switch[ptr_now + 3] == "-")
                                        I_value[found] -= I_temp;
                                    else if (string_switch[ptr_now + 3] == "~")
                                        I_value[found] *= I_temp;
                                    else if (string_switch[ptr_now + 3] == "_")
                                    {
                                        if (I_temp != 0)
                                            I_value[found] /= I_temp;
                                        else { err_message = Err_type.DEVIDED_ZERO; }
                                    }
                                    else
                                        I_value[found] = I_temp;
                                }
                                else { err_message = Err_type.UNKNOWN_VAR; }
                            }
                            else { err_message = Err_type.UNKNOWN_VAR; }

                            if (string_switch[ptr_now + 3] == "}" || string_switch[ptr_now + 3] == ":"
                                || string_switch[ptr_now + 3] == "Find")
                                r += 2;
                            else
                                r += 3;//跳到@I之後
                        }
                        else if (str_next_2 == "@F")
                        {
                            int amount = F_name.Count;
                            if (amount > 0)
                            {
                                int found = -1;
                                for (int I = 0; I < amount; I++)
                                {
                                    if (F_name[I] == string_switch[ptr_now + 1])
                                    {
                                        found = I; break;
                                    }
                                }
                                if (found != -1)
                                {
                                    if (string_switch[ptr_now - 3] == "@I" || string_switch[ptr_now - 2] == "@I")
                                    {
                                            int amount_in = I_name.Count;
                                        if(string_switch[ptr_now - 3] == "@I")
                                        {
                                            string int_name_4 = string_switch[ptr_now - 4];

                                            if (amount_in > 0)
                                            {
                                                int found_in = -1;
                                                for (int I = 0; I < amount_in; I++)
                                                {
                                                    if (I_name[I] == int_name_4)
                                                    {
                                                        found_in = I; break;
                                                    }
                                                }
                                                if (found_in != -1)
                                                {
                                                    F_temp = Convert.ToDouble(I_value[found_in]);
                                                }
                                            }
                                        }
                                        else if (string_switch[ptr_now - 2] == "@I")
                                        {
                                            string int_name_3 = string_switch[ptr_now - 3];
                                            
                                            if (amount_in > 0)
                                            {
                                                int found_in = -1;
                                                for (int I = 0; I < amount_in; I++)
                                                {
                                                    if (I_name[I] == int_name_3)
                                                    {
                                                        found_in = I; break;
                                                    }
                                                }
                                                if (found_in != -1)
                                                {
                                                    F_temp = Convert.ToDouble( I_value[found_in] );
                                                }
                                            }
                                        }
                                    }
                                    if (string_switch[ptr_now + 3] == "+")
                                        F_value[found] += F_temp;
                                    else if (string_switch[ptr_now + 3] == "-")
                                        F_value[found] -= F_temp;
                                    else if (string_switch[ptr_now + 3] == "~")
                                        F_value[found] *= F_temp;
                                    else if (string_switch[ptr_now + 3] == "_")
                                    {
                                        if (Math.Abs(F_temp) > 1e-8)
                                            F_value[found] /= F_temp;
                                        else { err_message = Err_type.DEVIDED_ZERO; }
                                    }
                                    else
                                        F_value[found] = F_temp;
                                }
                                else { err_message = Err_type.UNKNOWN_VAR; }
                            }
                            else { err_message = Err_type.UNKNOWN_VAR; }
                            //跳到@F之後
                            if (string_switch[ptr_now + 3] == "}" || string_switch[ptr_now + 3] == ":")
                                r += 2;
                            else
                                r += 3;
                        }
                        else if (str_next_2 == "@C")
                        {
                            int amount = C_name.Count;
                            if (amount > 0)
                            {
                                int found = -1;
                                for (int I = 0; I < amount; I++)
                                {
                                    if (C_name[I] == string_switch[ptr_now + 1])
                                    {
                                        found = I; break;
                                    }
                                }
                                if (found != -1)
                                {
                                    Complex Ctotal = new Complex();
                                    if (string_switch[ptr_now + 3] == "+")
                                    {
                                        Ctotal.R = C_temp.R + C_value[found].R;
                                        Ctotal.I = C_temp.I + C_value[found].I;
                                        C_value[found] = Ctotal;
                                    }
                                    else if (string_switch[ptr_now + 3] == "-")
                                    {
                                        Ctotal.R = C_value[found].R - C_temp.R;
                                        Ctotal.I = C_value[found].I - C_temp.I;
                                        C_value[found] = Ctotal;
                                    }
                                    else if (string_switch[ptr_now + 3] == "~")
                                    {
                                        Ctotal.R = C_value[found].R * C_temp.R - C_value[found].I * C_temp.I;
                                        Ctotal.I = C_value[found].I * C_temp.R + C_value[found].R * C_temp.I;
                                        C_value[found] = Ctotal;
                                    }
                                    else if (string_switch[ptr_now + 3] == "_")
                                    {
                                        double norm = C_temp.R * C_temp.R + C_temp.I * C_temp.I;
                                        if (norm > 0)
                                        {
                                            double reverse = 1.0 / norm;
                                            Ctotal.R = (C_value[found].R * C_temp.R + C_value[found].I * C_temp.I) * reverse;
                                            Ctotal.I = (C_value[found].I * C_temp.R - C_value[found].R * C_temp.I) * reverse;
                                            C_value[found] = Ctotal;
                                        }
                                        else { err_message = Err_type.DEVIDED_ZERO; }
                                    }
                                    else
                                        C_value[found] = C_temp;
                                }
                                else { err_message = Err_type.UNKNOWN_VAR; }
                            }
                            else { err_message = Err_type.UNKNOWN_VAR; }

                            if (string_switch[ptr_now + 3] == "}" || string_switch[ptr_now + 3] == ":")
                                r += 2;
                            else
                                r += 3;//跳到@C之後
                        }
                        else if (str_next_2 == "@Q")
                        {
                            int amount = Q_name.Count;
                            if (amount > 0)
                            {
                                int found = -1;
                                for (int I = 0; I < amount; I++)
                                {
                                    if (Q_name[I] == string_switch[ptr_now + 1])
                                    {
                                        found = I; break;
                                    }
                                }
                                if (found != -1)
                                {
                                    Quaternion Qtotal = new Quaternion();
                                    if (string_switch[ptr_now + 3] == "+")
                                    {
                                        Qtotal.R = Q_temp.R + Q_value[found].R;
                                        Qtotal.I = Q_temp.I + Q_value[found].I;
                                        Q_value[found] = Qtotal;
                                    }
                                    else if (string_switch[ptr_now + 3] == "-")
                                    {
                                        Qtotal.R = Q_value[found].R - Q_temp.R;
                                        Qtotal.I = Q_value[found].I - Q_temp.I;
                                        Q_value[found] = Qtotal;
                                    }
                                    else if (string_switch[ptr_now + 3] == "~")
                                    {
                                        Qtotal.R = Q_value[found].R * Q_temp.R - Q_value[found].I * Q_temp.I - Q_value[found].J * Q_temp.J - Q_value[found].K * Q_temp.K;
                                        Qtotal.I = Q_value[found].R * Q_temp.I + Q_value[found].I * Q_temp.R + Q_value[found].J * Q_temp.K - Q_value[found].K * Q_temp.J;
                                        Qtotal.J = Q_value[found].R * Q_temp.J + Q_value[found].J * Q_temp.R + Q_value[found].K * Q_temp.I - Q_value[found].I * Q_temp.K;
                                        Qtotal.K = Q_value[found].R * Q_temp.K + Q_value[found].K * Q_temp.R + Q_value[found].I * Q_temp.J - Q_value[found].J * Q_temp.I;
                                        Q_value[found] = Qtotal;
                                    }
                                    else if (string_switch[ptr_now + 3] == "_")
                                    {
                                        double norm = Q_temp.R * Q_temp.R + Q_temp.I * Q_temp.I + Q_temp.J * Q_temp.J + Q_temp.K * Q_temp.K;
                                        if (norm > 0)
                                        {
                                            double reverse = 1.0 / norm;
                                            Qtotal.R = reverse * (Q_value[found].R * Q_temp.R + Q_value[found].I * Q_temp.I + Q_value[found].J * Q_temp.J + Q_value[found].K * Q_temp.K);
                                            Qtotal.I = reverse * (-Q_value[found].R * Q_temp.I + Q_value[found].I * Q_temp.R - Q_value[found].J * Q_temp.K + Q_value[found].K * Q_temp.J);
                                            Qtotal.J = reverse * (-Q_value[found].R * Q_temp.J + Q_value[found].J * Q_temp.R - Q_value[found].K * Q_temp.I + Q_value[found].I * Q_temp.K);
                                            Qtotal.K = reverse * (-Q_value[found].R * Q_temp.K + Q_value[found].K * Q_temp.R - Q_value[found].I * Q_temp.J + Q_value[found].J * Q_temp.I);
                                            Q_value[found] = Qtotal;
                                        }
                                        else { err_message = Err_type.DEVIDED_ZERO; }
                                    }
                                    else
                                        C_value[found] = C_temp;
                                }
                                else { err_message = Err_type.UNKNOWN_VAR; }
                            }
                            else { err_message = Err_type.UNKNOWN_VAR; }

                            if (string_switch[ptr_now + 3] == "}" || string_switch[ptr_now + 3] == ":")
                                r += 2;
                            else
                                r += 3;//跳到@Q之後
                        }
                    }
                    else if (str_now == "||") //等於
                    {
                        if (input1 == "@I")
                        {
                                int amount = I_name.Count;
                                if (amount > 0)
                                {
                                    int found = -1;
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (I_name[I] == string_switch[ptr_now - 2])
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (found != -1)
                                    {
                                        I_temp = I_value[found];
                                    }
                                }
                                else { err_message = Err_type.UNKNOWN_VAR; }
                        }
                        else if (input1 == "@F")
                            {
                                int amount = F_name.Count;
                                if (amount > 0)
                                {
                                    int found = -1;
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (F_name[I] == string_switch[ptr_now - 2])
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (found != -1)
                                    {
                                        F_temp = F_value[found];
                                    }
                                    else { err_message = Err_type.UNKNOWN_VAR; }
                                }
                            }
                        else if (input1 == "@C")
                        {
                            int amount = C_name.Count;
                            if (amount > 0)
                            {
                                int found = -1;
                                for (int I = 0; I < amount; I++)
                                {
                                    if (C_name[I] == string_switch[ptr_now - 2])
                                    {
                                        found = I; break;
                                    }
                                }
                                if (found != -1)
                                {
                                    C_temp.R = C_value[found].R ;
                                    C_temp.I = C_value[found].I ;
                                }
                                else { err_message = Err_type.UNKNOWN_VAR; }
                            }
                        }
                        else if (input1 == "@Q")
                        {
                            int amount = Q_name.Count;
                            if (amount > 0)
                            {
                                int found = -1;
                                for (int I = 0; I < amount; I++)
                                {
                                    if (Q_name[I] == string_switch[ptr_now - 2])
                                    {
                                        found = I; break;
                                    }
                                }
                                if (found != -1)
                                {
                                    Q_temp.R = Q_value[found].R ;
                                    Q_temp.I = Q_value[found].I ;
                                    Q_temp.J = Q_value[found].J ;
                                    Q_temp.K = Q_value[found].K ;
                                }
                                else { err_message = Err_type.UNKNOWN_VAR; }
                            }
                        }
                        
                    }
                    else if (str_now == "+")
                    {
                        if (is_int2)
                        {
                            if (is_int1)
                            {
                                I_temp = int1 + int2;
                            }
                            else if (input1 == "@I")
                            {
                                int amount = I_name.Count;
                                if (amount > 0)
                                {
                                    int found = -1;
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (I_name[I] == string_switch[ptr_now - 2])
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (found != -1)
                                    {
                                        I_value[found] += int2;
                                        I_temp = I_value[found];
                                    }
                                    else { err_message = Err_type.UNKNOWN_VAR; }
                                }
                            }
                            else { err_message = Err_type.TYPE_ERR; }
                        }
                        else if (is_flo2)
                        {
                            if (is_flo1)
                            {
                                F_temp = flo1 + flo2;
                            }
                            else if (input1 == "@F")
                            {
                                int amount = F_name.Count;
                                if (amount > 0)
                                {
                                    int found = -1;
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (F_name[I] == string_switch[ptr_now - 2])
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (found != -1)
                                    {
                                        F_value[found] += flo2;
                                        F_temp = F_value[found];
                                    }
                                    else { err_message = Err_type.UNKNOWN_VAR; }
                                }
                            }
                            else { err_message = Err_type.TYPE_ERR; }
                        }
                        else if (is_com2)
                        {
                            if (is_com1)
                            {
                                C_temp.R = com1.R + com2.R;
                                C_temp.I = com1.I + com2.I;
                            }
                            else if (input1 == "@C")
                            {
                                int amount = C_name.Count;
                                if (amount > 0)
                                {
                                    int found = -1;
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (C_name[I] == string_switch[ptr_now - 2])
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (found != -1)
                                    {
                                        C_temp.R = C_value[found].R + com2.R;
                                        C_temp.I = C_value[found].I + com2.I;
                                        C_value[found] = C_temp;
                                    }
                                    else { err_message = Err_type.UNKNOWN_VAR; }
                                }
                            }
                            else { err_message = Err_type.TYPE_ERR; }
                        }
                        else if (is_qua2)
                        {
                            if (is_qua1)
                            {
                                Q_temp.R = qua1.R + qua2.R;
                                Q_temp.I = qua1.I + qua2.I;
                                Q_temp.J = qua1.J + qua2.J;
                                Q_temp.K = qua1.K + qua2.K;
                            }
                            else if (input1 == "@Q")
                            {
                                int amount = Q_name.Count;
                                if (amount > 0)
                                {
                                    int found = -1;
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (Q_name[I] == string_switch[ptr_now - 2])
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (found != -1)
                                    {
                                        Q_temp.R = Q_value[found].R + qua2.R;
                                        Q_temp.I = Q_value[found].I + qua2.I;
                                        Q_temp.J = Q_value[found].J + qua2.J;
                                        Q_temp.K = Q_value[found].K + qua2.K;
                                        Q_value[found] = Q_temp;
                                    }
                                    else { err_message = Err_type.UNKNOWN_VAR; }
                                }
                            }
                            else { err_message = Err_type.TYPE_ERR; }
                        }
                        else
                        {
                            if (input1 == "@S")
                            {
                                int amount = S_name.Count;
                                if (amount > 0)
                                {
                                    int found = -1;
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (S_name[I] == string_switch[ptr_now - 2])
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (found != -1)
                                    {
                                        S_value[found] += input2;
                                        S_temp = S_value[found];
                                    }
                                    else { err_message = Err_type.UNKNOWN_VAR; }
                                }
                            }
                            else
                                S_temp = input1 + input2;
                        }
                        r += 1;
                    }
                    else if (str_now == "-")
                    {
                        if (is_int2)
                        {
                            if (is_int1)
                            {
                                I_temp = int1 - int2;
                            }
                            else if (input1 == "@I")
                            {
                                int amount = I_name.Count;
                                if (amount > 0)
                                {
                                    int found = -1;
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (I_name[I] == string_switch[ptr_now - 2])
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (found != -1)
                                    {
                                        I_value[found] -= int2;
                                        I_temp = I_value[found];
                                    }
                                    else { err_message = Err_type.UNKNOWN_VAR; }
                                }
                            }
                            else { err_message = Err_type.TYPE_ERR; }
                        }
                        else if (is_flo2)
                        {
                            if (is_flo1)
                            {
                                F_temp = flo1 - flo2;
                            }
                            else if (input1 == "@F")
                            {
                                int amount = F_name.Count;
                                if (amount > 0)
                                {
                                    int found = -1;
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (F_name[I] == string_switch[ptr_now - 2])
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (found != -1)
                                    {
                                        F_value[found] -= flo2;
                                        F_temp = F_value[found];
                                    }
                                    else { err_message = Err_type.UNKNOWN_VAR; }
                                }
                            }
                            else { err_message = Err_type.TYPE_ERR; }
                        }
                        else if (is_com2)
                        {
                            if (is_com1)
                            {
                                C_temp.R = com1.R - com2.R;
                                C_temp.I = com1.I - com2.I;
                            }
                            else if (input1 == "@C")
                            {
                                int amount = C_name.Count;
                                if (amount > 0)
                                {
                                    int found = -1;
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (C_name[I] == string_switch[ptr_now - 2])
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (found != -1)
                                    {
                                        C_temp.R = C_value[found].R - com2.R;
                                        C_temp.I = C_value[found].I - com2.I;
                                        C_value[found] = C_temp;
                                    }
                                    else { err_message = Err_type.UNKNOWN_VAR; }
                                }
                            }
                            else { err_message = Err_type.TYPE_ERR; }
                        }
                        else if (is_qua2)
                        {
                            if (is_qua1)
                            {
                                Q_temp.R = qua1.R - qua2.R;
                                Q_temp.I = qua1.I - qua2.I;
                                Q_temp.J = qua1.J - qua2.J;
                                Q_temp.K = qua1.K - qua2.K;
                            }
                            else if (input1 == "@Q")
                            {
                                int amount = Q_name.Count;
                                if (amount > 0)
                                {
                                    int found = -1;
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (Q_name[I] == string_switch[ptr_now - 2])
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (found != -1)
                                    {
                                        Q_temp.R = Q_value[found].R - qua2.R;
                                        Q_temp.I = Q_value[found].I - qua2.I;
                                        Q_temp.J = Q_value[found].J - qua2.J;
                                        Q_temp.K = Q_value[found].K - qua2.K;
                                        Q_value[found] = Q_temp;
                                    }
                                    else { err_message = Err_type.UNKNOWN_VAR; }
                                }
                            }
                            else { err_message = Err_type.TYPE_ERR; }
                        }
                        else { err_message = Err_type.TYPE_ERR; }
                        r += 1;
                    }
                    else if (str_now == "~")
                    {
                        if (is_int2)
                        {
                            if (is_int1)
                            {
                                I_temp = int1 * int2;
                            }
                            else if (input1 == "@I")
                            {
                                int amount = I_name.Count;
                                if (amount > 0)
                                {
                                    int found = -1;
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (I_name[I] == string_switch[ptr_now - 2])
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (found != -1)
                                    {
                                        I_value[found] *= int2;
                                        I_temp = I_value[found];
                                    }
                                    else { err_message = Err_type.UNKNOWN_VAR; }
                                }
                            }
                            else { err_message = Err_type.TYPE_ERR; }
                        }
                        else if (is_flo2)
                        {
                            if (is_flo1)
                            {
                                F_temp = flo1 * flo2;
                            }
                            else if (input1 == "@F")
                            {
                                int amount = F_name.Count;
                                if (amount > 0)
                                {
                                    int found = -1;
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (F_name[I] == string_switch[ptr_now - 2])
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (found != -1)
                                    {
                                        F_value[found] *= flo2;
                                        F_temp = F_value[found];
                                    }
                                    else { err_message = Err_type.UNKNOWN_VAR; }
                                }
                            }
                            else { err_message = Err_type.TYPE_ERR; }
                        }
                        else if (is_com2)
                        {
                            if (is_com1)
                            {
                                C_temp.R = com1.R * com2.R - com1.I * com2.I;
                                C_temp.I = com1.I * com2.R + com1.R * com2.I;
                            }
                            else if (input1 == "@C")
                            {
                                int amount = C_name.Count;
                                if (amount > 0)
                                {
                                    int found = -1;
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (C_name[I] == string_switch[ptr_now - 2])
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (found != -1)
                                    {
                                        C_temp.R = C_value[found].R * com2.R - C_value[found].I * com2.I;
                                        C_temp.I = C_value[found].I * com2.R + C_value[found].R * com2.I;
                                        C_value[found] = C_temp;
                                    }
                                    else { err_message = Err_type.UNKNOWN_VAR; }
                                }
                            }
                            else { err_message = Err_type.TYPE_ERR; }
                        }
                        else if (is_qua2)
                        {
                            if (is_qua1)
                            {
                                Q_temp.R = qua1.R * qua2.R - qua1.I * qua2.I - qua1.J * qua2.J - qua1.K * qua2.K;
                                Q_temp.I = qua1.R * qua2.I + qua1.I * qua2.R + qua1.J * qua2.K - qua1.K * qua2.J;
                                Q_temp.J = qua1.R * qua2.J + qua1.J * qua2.R + qua1.K * qua2.I - qua1.I * qua2.K;
                                Q_temp.K = qua1.R * qua2.K + qua1.K * qua2.R + qua1.I * qua2.J - qua1.J * qua2.I;
                            }
                            else if (input1 == "@Q")
                            {
                                int amount = Q_name.Count;
                                if (amount > 0)
                                {
                                    int found = -1;
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (Q_name[I] == string_switch[ptr_now - 2])
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (found != -1)
                                    {
                                        Q_temp.R = Q_value[found].R * qua2.R - Q_value[found].I * qua2.I - Q_value[found].J * qua2.J - Q_value[found].K * qua2.K;
                                        Q_temp.I = Q_value[found].R * qua2.I + Q_value[found].I * qua2.R + Q_value[found].J * qua2.K - Q_value[found].K * qua2.J;
                                        Q_temp.J = Q_value[found].R * qua2.J + Q_value[found].J * qua2.R + Q_value[found].K * qua2.I - Q_value[found].I * qua2.K;
                                        Q_temp.K = Q_value[found].R * qua2.K + Q_value[found].K * qua2.R + Q_value[found].I * qua2.J - Q_value[found].J * qua2.I;
                                        Q_value[found] = Q_temp;
                                    }
                                    else { err_message = Err_type.UNKNOWN_VAR; }
                                }
                            }
                            else { err_message = Err_type.TYPE_ERR; }
                        }
                        else { err_message = Err_type.TYPE_ERR; }
                        r += 1;
                    }
                    else if (str_now == "_")
                    {
                        if (is_int2)
                        {
                            if (int2 != 0)
                                if (is_int1)
                                {
                                    I_temp = int1 / int2;
                                }
                                else if (input1 == "@I")
                                {
                                    int amount = I_name.Count;
                                    if (amount > 0)
                                    {
                                        int found = -1;
                                        for (int I = 0; I < amount; I++)
                                        {
                                            if (I_name[I] == string_switch[ptr_now - 2])
                                            {
                                                found = I; break;
                                            }
                                        }
                                        if (found != -1)
                                        {
                                            I_value[found] /= int2;
                                            I_temp = I_value[found];
                                        }
                                        else { err_message = Err_type.UNKNOWN_VAR; }
                                    }
                                }
                                else { err_message = Err_type.TYPE_ERR; }
                            else { err_message = Err_type.DEVIDED_ZERO; }
                        }
                        else if (is_flo2)
                        {
                            if (+flo2 > 0.0)
                                if (is_flo1)
                                {
                                    F_temp = flo1 / flo2;
                                }
                                else if (input1 == "@F")
                                {
                                    int amount = F_name.Count;
                                    if (amount > 0)
                                    {
                                        int found = -1;
                                        for (int I = 0; I < amount; I++)
                                        {
                                            if (F_name[I] == string_switch[ptr_now - 2])
                                            {
                                                found = I; break;
                                            }
                                        }
                                        if (found != -1)
                                        {
                                            F_value[found] /= flo2;
                                            F_temp = F_value[found];
                                        }
                                        else { err_message = Err_type.UNKNOWN_VAR; }
                                    }
                                }
                                else { err_message = Err_type.TYPE_ERR; }
                            else { err_message = Err_type.DEVIDED_ZERO; }
                        }
                        else if (is_com2)
                        {
                            double norm = com2.R * com2.R + com2.I * com2.I;
                            if (norm > 0.0)
                                if (is_com1)
                                {
                                    double reverse = 1.0 / norm;
                                    C_temp.R = (com1.R * com2.R + com1.I * com2.I) * reverse;
                                    C_temp.I = (com1.I * com2.R - com1.R * com2.I) * reverse;
                                }
                                else if (input1 == "@C")
                                {
                                    int amount = C_name.Count;
                                    if (amount > 0)
                                    {
                                        int found = -1;
                                        for (int I = 0; I < amount; I++)
                                        {
                                            if (C_name[I] == string_switch[ptr_now - 2])
                                            {
                                                found = I; break;
                                            }
                                        }
                                        if (found != -1)
                                        {
                                            double reverse = 1.0 / norm;
                                            C_temp.R = (C_value[found].R * com2.R + C_value[found].I * com2.I) * reverse;
                                            C_temp.I = (C_value[found].I * com2.R - C_value[found].R * com2.I) * reverse;
                                            C_value[found] = C_temp;
                                        }
                                        else { err_message = Err_type.UNKNOWN_VAR; }
                                    }
                                }
                                else { err_message = Err_type.TYPE_ERR; }
                            else { err_message = Err_type.DEVIDED_ZERO; }
                        }
                        else if (is_qua2)
                        {
                            double norm = qua2.R * qua2.R + qua2.I * qua2.I + qua2.J * qua2.J + qua2.K * qua2.K;
                            if (norm > 0.0)
                                if (is_qua1)
                                {
                                    double reverse = 1.0 / norm;
                                    Q_temp.R = reverse * (qua1.R * qua2.R + qua1.I * qua2.I + qua1.J * qua2.J + qua1.K * qua2.K);
                                    Q_temp.I = reverse * (-qua1.R * qua2.I + qua1.I * qua2.R - qua1.J * qua2.K + qua1.K * qua2.J);
                                    Q_temp.J = reverse * (-qua1.R * qua2.J + qua1.J * qua2.R - qua1.K * qua2.I + qua1.I * qua2.K);
                                    Q_temp.K = reverse * (-qua1.R * qua2.K + qua1.K * qua2.R - qua1.I * qua2.J + qua1.J * qua2.I);
                                }
                                else if (input1 == "@Q")
                                {
                                    int amount = Q_name.Count;
                                    if (amount > 0)
                                    {
                                        int found = -1;
                                        for (int I = 0; I < amount; I++)
                                        {
                                            if (Q_name[I] == string_switch[ptr_now - 2])
                                            {
                                                found = I; break;
                                            }
                                        }
                                        if (found != -1)
                                        {
                                            double reverse = 1.0 / norm;
                                            Q_temp.R = reverse * (Q_value[found].R * qua2.R + Q_value[found].I * qua2.I + Q_value[found].J * qua2.J + Q_value[found].K * qua2.K);
                                            Q_temp.I = reverse * (-Q_value[found].R * qua2.I + Q_value[found].I * qua2.R - Q_value[found].J * qua2.K + Q_value[found].K * qua2.J);
                                            Q_temp.J = reverse * (-Q_value[found].R * qua2.J + Q_value[found].J * qua2.R - Q_value[found].K * qua2.I + Q_value[found].I * qua2.K);
                                            Q_temp.K = reverse * (-Q_value[found].R * qua2.K + Q_value[found].K * qua2.R - Q_value[found].I * qua2.J + Q_value[found].J * qua2.I);
                                            Q_value[found] = Q_temp;
                                        }
                                        else { err_message = Err_type.UNKNOWN_VAR; }
                                    }
                                }
                                else { err_message = Err_type.TYPE_ERR; }
                            else { err_message = Err_type.DEVIDED_ZERO; }
                        }
                        else { err_message = Err_type.TYPE_ERR; }
                        r += 1;
                    }
                }
                #endregion
                #region 流程控制
                    //because判斷式，格式 `,` (and & or ^)"statement" ,`, & ... 
                    //or because % `,` ... % & ... else % ... end because ,`,
                    //recursive判斷式 `,` "statement" ,`, & ... ^
                    //"statement"判定條件：小於<大於>大於等於/<小於等於/>不等!=等於=
                else if (str_now == "Abstract")
                {
                    Abstra = r;
                }
                else if (str_now == "Introduction")
                {
                    Intro = r;
                }
                else if (str_now == "`,`")
                {
                    int and_or = 0;
                    int cond_end; //條件敘述的尾端
                    ptr_now = r;
                    for (cond_end = ptr_now; cond_end < code_length; cond_end++)
                    {
                        if (string_switch[cond_end] == ",`,")
                            break;
                    }
                    for (int now = ptr_now; now < cond_end; now++)
                    {
                        if (string_switch[now] == "&" || string_switch[now] == "^")
                        {
                            if (string_switch[now] == "&")
                                and_or = 1;
                            else if (string_switch[now] == "^")
                                and_or = 2;
                            now++;
                        }
                        else if (string_switch[now] == ">" || string_switch[now] == "<" ||
                            string_switch[now] == "/<" || string_switch[now] == "/>" ||
                            string_switch[now] == "/=" || string_switch[now] == "=")
                        {
                            if (string_switch[now] == ">")
                            {
                                if (string_switch[now - 1] == "@I")
                                {
                                    string next = string_switch[now + 1], last = string_switch[now - 2];
                                    int next_out = 0;
                                    int amount = I_name.Count;
                                    int found = -1;
                                    bool is_int = int.TryParse(next, out next_out);
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (I_name[I] == last)
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (amount > 0 && found >= 0 && is_int)
                                    {
                                        if (I_value[found] > next_out)
                                            B_temp = true;
                                        else
                                            B_temp = false;
                                        if (and_or == 1)
                                            B_total = B_total && B_temp;
                                        else if (and_or == 2)
                                            B_total = B_total || B_temp;
                                        else
                                            B_total = B_temp;
                                    }
                                }
                                else if (string_switch[now - 1] == "@F")
                                {
                                    string next = string_switch[now + 1], last = string_switch[now - 2];
                                    Double next_out = 0.0f;
                                    int amount = F_name.Count;
                                    int found = -1;
                                    bool is_flo = Double.TryParse(next, out next_out);
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (F_name[I] == last)
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (amount > 0 && found >= 0 && is_flo)
                                    {
                                        if (F_value[found] > next_out)
                                            B_temp = true;
                                        else
                                            B_temp = false;
                                        if (and_or == 1)
                                            B_total = B_total && B_temp;
                                        else if (and_or == 2)
                                            B_total = B_total || B_temp;
                                        else
                                            B_total = B_temp;
                                    }
                                }
                                else { err_message = Err_type.SYNTEX_ERR; }
                            }
                            else if (string_switch[now] == "<")
                            {
                                if (string_switch[now - 1] == "@I")
                                {
                                    string next = string_switch[now + 1], last = string_switch[now - 2];
                                    int next_out = 0;
                                    int amount = I_name.Count;
                                    int found = -1;
                                    bool is_int = int.TryParse(next, out next_out);
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (I_name[I] == last)
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (amount > 0 && found >= 0 && is_int)
                                    {
                                        if (I_value[found] < next_out)
                                            B_temp = true;
                                        else
                                            B_temp = false;
                                        if (and_or == 1)
                                            B_total = B_total && B_temp;
                                        else if (and_or == 2)
                                            B_total = B_total || B_temp;
                                        else
                                            B_total = B_temp;
                                    }
                                }
                                else if (string_switch[now - 1] == "@F")
                                {
                                    string next = string_switch[now + 1], last = string_switch[now - 2];
                                    Double next_out = 0.0f;
                                    int amount = F_name.Count;
                                    int found = -1;
                                    bool is_flo = Double.TryParse(next, out next_out);
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (F_name[I] == last)
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (amount > 0 && found >= 0 && is_flo)
                                    {
                                        if (F_value[found] < next_out)
                                            B_temp = true;
                                        else
                                            B_temp = false;
                                        if (and_or == 1)
                                            B_total = B_total && B_temp;
                                        else if (and_or == 2)
                                            B_total = B_total || B_temp;
                                        else
                                            B_total = B_temp;
                                    }
                                }
                                else { err_message = Err_type.SYNTEX_ERR; }
                            }
                            else if (string_switch[now] == "/<")
                            {
                                if (string_switch[now - 1] == "@I")
                                {
                                    string next = string_switch[now + 1], last = string_switch[now - 2];
                                    int next_out = 0;
                                    int amount = I_name.Count;
                                    int found = -1;
                                    bool is_int = int.TryParse(next, out next_out);
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (I_name[I] == last)
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (amount > 0 && found >= 0 && is_int)
                                    {
                                        if (I_value[found] >= next_out)
                                            B_temp = true;
                                        else
                                            B_temp = false;
                                        if (and_or == 1)
                                            B_total = B_total && B_temp;
                                        else if (and_or == 2)
                                            B_total = B_total || B_temp;
                                        else
                                            B_total = B_temp;
                                    }
                                }
                                else if (string_switch[now - 1] == "@F")
                                {
                                    string next = string_switch[now + 1], last = string_switch[now - 2];
                                    Double next_out = 0.0f;
                                    int amount = F_name.Count;
                                    int found = -1;
                                    bool is_flo = Double.TryParse(next, out next_out);
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (F_name[I] == last)
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (amount > 0 && found >= 0 && is_flo)
                                    {
                                        if (F_value[found] >= next_out)
                                            B_temp = true;
                                        else
                                            B_temp = false;
                                        if (and_or == 1)
                                            B_total = B_total && B_temp;
                                        else if (and_or == 2)
                                            B_total = B_total || B_temp;
                                        else
                                            B_total = B_temp;
                                    }
                                }
                                else { err_message = Err_type.SYNTEX_ERR; }
                            }
                            else if (string_switch[now] == "/>")
                            {
                                if (string_switch[now - 1] == "@I")
                                {
                                    string next = string_switch[now + 1], last = string_switch[now - 2];
                                    int next_out = 0;
                                    int amount = I_name.Count;
                                    int found = -1;
                                    bool is_int = int.TryParse(next, out next_out);
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (I_name[I] == last)
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (amount > 0 && found >= 0 && is_int)
                                    {
                                        if (I_value[found] <= next_out)
                                            B_temp = true;
                                        else
                                            B_temp = false;
                                        if (and_or == 1)
                                            B_total = B_total && B_temp;
                                        else if (and_or == 2)
                                            B_total = B_total || B_temp;
                                        else
                                            B_total = B_temp;
                                    }
                                }
                                else if (string_switch[now - 1] == "@F")
                                {
                                    string next = string_switch[now + 1], last = string_switch[now - 2];
                                    Double next_out = 0.0f;
                                    int amount = F_name.Count;
                                    int found = -1;
                                    bool is_flo = Double.TryParse(next, out next_out);
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (F_name[I] == last)
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (amount > 0 && found >= 0 && is_flo)
                                    {
                                        if (F_value[found] <= next_out)
                                            B_temp = true;
                                        else
                                            B_temp = false;
                                        if (and_or == 1)
                                            B_total = B_total && B_temp;
                                        else if (and_or == 2)
                                            B_total = B_total || B_temp;
                                        else
                                            B_total = B_temp;
                                    }
                                }
                                else { err_message = Err_type.SYNTEX_ERR; }
                            }
                            else { err_message = Err_type.SYNTEX_ERR; }
                            now++;
                        }
                        else if (string_switch[now] == "/=" || string_switch[now] == "=")
                        {
                            if (string_switch[now] == "/=")
                            {
                                if (string_switch[now - 1] == "@I")
                                {
                                    string next = string_switch[now + 1], last = string_switch[now - 2];
                                    int next_out = 0;
                                    int amount = I_name.Count;
                                    int found = -1;
                                    bool is_int = int.TryParse(next, out next_out);
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (I_name[I] == last)
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (amount > 0 && found >= 0 && is_int)
                                    {
                                        if (I_value[found] != next_out)
                                            B_temp = true;
                                        else
                                            B_temp = false;
                                        if (and_or == 1)
                                            B_total = B_total && B_temp;
                                        else if (and_or == 2)
                                            B_total = B_total || B_temp;
                                        else
                                            B_total = B_temp;
                                    }
                                }
                                else if (string_switch[now - 1] == "@F")
                                {
                                    string next = string_switch[now + 1], last = string_switch[now - 2];
                                    Double next_out = 0.0f;
                                    int amount = F_name.Count;
                                    int found = -1;
                                    bool is_flo = Double.TryParse(next, out next_out);
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (F_name[I] == last)
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (amount > 0 && found >= 0 && is_flo)
                                    {
                                        if (+(F_value[found] - next_out) > 1e-4)
                                            B_temp = true;
                                        else
                                            B_temp = false;
                                        if (and_or == 1)
                                            B_total = B_total && B_temp;
                                        else if (and_or == 2)
                                            B_total = B_total || B_temp;
                                        else
                                            B_total = B_temp;
                                    }
                                }
                                else if (string_switch[now - 1] == "@C")
                                {
                                    string next = string_switch[now + 1], last = string_switch[now - 2];
                                    Complex next_out = new Complex();
                                    int amount = C_name.Count;
                                    int found = -1;
                                    bool is_com = Complex_TryParse(next, out next_out);
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (C_name[I] == last)
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (amount > 0 && found >= 0 && is_com)
                                    {
                                        if (+(C_value[found].R - next_out.R) > 1e-9 ||
                                            +(C_value[found].I - next_out.I) > 1e-9)
                                            B_temp = true;
                                        else
                                            B_temp = false;
                                        if (and_or == 1)
                                            B_total = B_total && B_temp;
                                        else if (and_or == 2)
                                            B_total = B_total || B_temp;
                                        else
                                            B_total = B_temp;
                                    }
                                }
                                else if (string_switch[now - 1] == "@Q")
                                {
                                    string next = string_switch[now + 1], last = string_switch[now - 2];
                                    Quaternion next_out = new Quaternion();
                                    int amount = C_name.Count;
                                    int found = -1;
                                    bool is_qua = Quaternion_TryParse(next, out next_out);
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (Q_name[I] == last)
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (amount > 0 && found >= 0 && is_qua)
                                    {
                                        if (+(Q_value[found].R - next_out.R) > 1e-9 ||
                                            +(Q_value[found].I - next_out.I) > 1e-9 ||
                                            +(Q_value[found].J - next_out.J) > 1e-9 ||
                                            +(Q_value[found].K - next_out.K) > 1e-9)
                                            B_temp = true;
                                        else
                                            B_temp = false;
                                        if (and_or == 1)
                                            B_total = B_total && B_temp;
                                        else if (and_or == 2)
                                            B_total = B_total || B_temp;
                                        else
                                            B_total = B_temp;
                                    }
                                }
                                else { err_message = Err_type.SYNTEX_ERR; }
                            }
                            else if (string_switch[now] == "=")
                            {
                                if (string_switch[now - 1] == "@I")
                                {
                                    string next = string_switch[now + 1], last = string_switch[now - 2];
                                    int next_out = 0;
                                    int amount = I_name.Count;
                                    int found = -1;
                                    bool is_int = int.TryParse(next, out next_out);
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (I_name[I] == last)
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (amount > 0 && found >= 0 && is_int)
                                    {
                                        if (I_value[found] == next_out)
                                            B_temp = true;
                                        else
                                            B_temp = false;
                                        if (and_or == 1)
                                            B_total = B_total && B_temp;
                                        else if (and_or == 2)
                                            B_total = B_total || B_temp;
                                        else
                                            B_total = B_temp;
                                    }
                                }
                                else if (string_switch[now - 1] == "@F")
                                {
                                    string next = string_switch[now + 1], last = string_switch[now - 2];
                                    Double next_out = 0.0f;
                                    int amount = F_name.Count;
                                    int found = -1;
                                    bool is_flo = Double.TryParse(next, out next_out);
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (F_name[I] == last)
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (amount > 0 && found >= 0 && is_flo)
                                    {
                                        if (+(F_value[found] - next_out) < 1e-4)
                                            B_temp = true;
                                        else
                                            B_temp = false;
                                        if (and_or == 1)
                                            B_total = B_total && B_temp;
                                        else if (and_or == 2)
                                            B_total = B_total || B_temp;
                                        else
                                            B_total = B_temp;
                                    }
                                }
                                else if (string_switch[now - 1] == "@C")
                                {
                                    string next = string_switch[now + 1], last = string_switch[now - 2];
                                    Complex next_out = new Complex();
                                    int amount = C_name.Count;
                                    int found = -1;
                                    bool is_com = Complex_TryParse(next, out next_out);
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (C_name[I] == last)
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (amount > 0 && found >= 0 && is_com)
                                    {
                                        if (+(C_value[found].R - next_out.R) < 1e-9 ||
                                            +(C_value[found].I - next_out.I) < 1e-9)
                                            B_temp = true;
                                        else
                                            B_temp = false;
                                        if (and_or == 1)
                                            B_total = B_total && B_temp;
                                        else if (and_or == 2)
                                            B_total = B_total || B_temp;
                                        else
                                            B_total = B_temp;
                                    }
                                }
                                else if (string_switch[now - 1] == "@Q")
                                {
                                    string next = string_switch[now + 1], last = string_switch[now - 2];
                                    Quaternion next_out = new Quaternion();
                                    int amount = C_name.Count;
                                    int found = -1;
                                    bool is_qua = Quaternion_TryParse(next, out next_out);
                                    for (int I = 0; I < amount; I++)
                                    {
                                        if (Q_name[I] == last)
                                        {
                                            found = I; break;
                                        }
                                    }
                                    if (amount > 0 && found >= 0 && is_qua)
                                    {
                                        if (+(Q_value[found].R - next_out.R) < 1e-9 ||
                                            +(Q_value[found].I - next_out.I) < 1e-9 ||
                                            +(Q_value[found].J - next_out.J) < 1e-9 ||
                                            +(Q_value[found].K - next_out.K) < 1e-9)
                                            B_temp = true;
                                        else
                                            B_temp = false;
                                        if (and_or == 1)
                                            B_total = B_total && B_temp;
                                        else if (and_or == 2)
                                            B_total = B_total || B_temp;
                                        else
                                            B_total = B_temp;
                                    }
                                }
                                else { err_message = Err_type.SYNTEX_ERR; }
                            }
                            now++;
                        }
                    }
                    r = cond_end;
                    //如果判斷式是正確的
                    if (B_total == true)
                    {
                        if (string_switch[r + 1] == "&")
                            continue;
                        else { err_message = Err_type.SYNTEX_ERR; }
                    }
                    else
                    {
                        int cuz_end; //條件敘述的尾端
                        ptr_now = r + 1;
                        for (cuz_end = ptr_now; cuz_end < code_length; cuz_end++)
                        {
                            if (string_switch[cuz_end] == ",`," || string_switch[cuz_end] == "^")
                                break;
                        }
                        r = cuz_end + 1;
                        if (string_switch[r] == "}")//如果迴圈發生在函數內
                        {
                            in_function = false;
                            r = fun_escape;
                        }
                    }
                }//`,`區段結尾
                //連結because中的TO DO事項
                else if (str_now == "&")
                {
                    continue;
                }
                //because中的TO DO事項結束
                else if (str_now == ",`,")
                {
                    if (string_switch[r + 1] == "%")//else if
                        r++;
                }
                //because作為迴圈
                else if (str_now == "^")
                {
                    int cuz_begin; //條件敘述的尾端
                    ptr_now = r;
                    for (cuz_begin = ptr_now; cuz_begin > 0; cuz_begin--)
                    {
                        if (string_switch[cuz_begin] == "`,`")
                            break;
                    }
                    r = cuz_begin - 1;
                }
                #endregion
                #region 函式宣告與引用
                //宣告格式 Define "name" ( "variant" @"type" ) { "TO DO" }
                //引用格式 Find "name" ( "variant" @"type" )
                else if (str_now == "Define")
                {
                    ptr_now = r;
                    string name = string_switch[ptr_now + 1],
                        start_of_var = string_switch[ptr_now + 2];
                    IndexOf_name = FUN_name.IndexOf(name);
                    if (IndexOf_name == -1 && start_of_var == "(") //宣告
                    {
                        int var_end, var_test = -1;
                        FUN_name.Add(name);
                        for (var_end = ptr_now; var_end < code_length; var_end++)
                        {
                            if (string_switch[var_end] == ")")
                            {
                                var_test = var_end;
                                break;
                            }
                        }
                        if (var_test != -1)
                        {
                            List<String> variant = new List<String>();
                            FUN_variant.Add(variant);
                            FUN_position.Add((uint)(var_test + 1));
                            int last_var = FUN_variant.Count;
                            for (int var_beg = ptr_now + 3; var_beg < var_end; var_beg++)
                            {
                                if (string_switch[var_beg] != "")
                                    FUN_variant[last_var - 1].Add(string_switch[var_beg]);
                            }
                            r = var_end - 1;
                        }
                        else { err_message = Err_type.SYNTEX_ERR; }
                    }
                }
                else if (str_now == "{")
                {
                    ptr_now = r;
                    int val_end, val_test = -1;
                    if (string_switch[ptr_now - 1] == ")")
                    {
                        List<String> value = new List<String>();
                        FUN_value.Add(value);
                        FUN_position.Add((uint)(ptr_now + 1));
                        int last_val = FUN_value.Count;
                        for (val_end = ptr_now + 1; val_end < code_length; val_end++)
                        {
                            if (string_switch[val_end] == "}")
                            {
                                val_test = val_end;
                                break;
                            }
                        }
                        if (val_test != -1)
                        {
                            for (int val_beg = ptr_now + 1; val_beg < val_end + 1; val_beg++)
                            {
                                if (string_switch[val_beg] != "")
                                    FUN_value[last_val - 1].Add(string_switch[val_beg]);
                            }
                            r = val_end;
                        }
                        else { err_message = Err_type.SYNTEX_ERR; }
                    }
                    else { err_message = Err_type.SYNTEX_ERR; }
                }
                else if (str_now == "}")
                {
                    if (in_function2 == true)
                    {
                        in_function2 = false;
                        in_function = true;
                        r = fun_escape2; //跳回上一個函式
                        Abstra = 0;
                    }
                    else if (in_function == true)
                    {
                        in_function = false;
                        in_tokens = true;
                        r = fun_escape; //跳回主程式
                        Abstra = Intro;
                    }
                    ptr_now = string_switch.Length;
                    int var_beg, var_test = -1;
                    for (var_beg = ptr_now - 1; var_beg > 0; var_beg--)
                    {
                        if (string_switch[var_beg] == "(")
                        {
                            var_test = var_beg + 1;
                            break;
                        }
                    }
                    if (var_test != -1)
                    {
                        int Index;
                        string name = string_switch[var_test - 2];
                        Index = FUN_name.IndexOf(name);

                        if (in_tokens) IndexOf_name = Index;
                        else if (in_function) IndexOf_name2 = Index;

                        List<String> variant = new List<String>();
                        for (; var_test < ptr_now; var_test++)
                        {
                            if (string_switch[var_test] == ")")
                                break;
                            else if (string_switch[var_test] != "")
                                variant.Add(string_switch[var_test]);
                        }
                        int variant_Count = variant.Count;
                        if (Index != -1)
                            if (FUN_variant[Index].Count == variant_Count && variant_Count % 2 == 0)
                            {
                                for (int pair = 0; pair < variant_Count / 2; pair++)
                                {
                                    int fun_var = -1, inp_var = -1;
                                    if (FUN_variant[Index][2 * pair + 1] == "@I")
                                    {
                                        for (int find_fun = 0; find_fun < I_name.Count; find_fun++)
                                        {
                                            if (I_name[find_fun] == FUN_variant[Index][2 * pair])
                                            {
                                                fun_var = find_fun; break;
                                            }
                                        }
                                        for (int find_fun = 0; find_fun < I_name.Count; find_fun++)
                                        {
                                            if (I_name[find_fun] == variant[2 * pair])
                                            {
                                                inp_var = find_fun; break;
                                            }
                                        }
                                        if (fun_var != -1 && inp_var != -1)
                                            I_value[inp_var] = I_value[fun_var];
                                        else { err_message = Err_type.UNKNOWN_VAR; }
                                    }
                                    else if (FUN_variant[Index][pair + 1] == "@F")
                                    {
                                        for (int find_fun = 0; find_fun < F_name.Count; find_fun++)
                                        {
                                            if (F_name[find_fun] == FUN_variant[Index][pair])
                                            {
                                                fun_var = find_fun; break;
                                            }
                                        }
                                        for (int find_fun = 0; find_fun < F_name.Count; find_fun++)
                                        {
                                            if (F_name[find_fun] == variant[pair])
                                            {
                                                inp_var = find_fun; break;
                                            }
                                        }
                                        if (fun_var != -1 && inp_var != -1)
                                            F_value[inp_var] = F_value[fun_var];
                                        else { err_message = Err_type.UNKNOWN_VAR; }
                                    }
                                    else if (FUN_variant[Index][pair + 1] == "@C")
                                    {
                                        for (int find_fun = 0; find_fun < C_name.Count; find_fun++)
                                        {
                                            if (C_name[find_fun] == FUN_variant[Index][pair])
                                            {
                                                fun_var = find_fun; break;
                                            }
                                        }
                                        for (int find_fun = 0; find_fun < C_name.Count; find_fun++)
                                        {
                                            if (C_name[find_fun] == variant[pair])
                                            {
                                                inp_var = find_fun; break;
                                            }
                                        }
                                        if (fun_var != -1 && inp_var != -1)
                                            C_value[inp_var] = C_value[fun_var];
                                        else { err_message = Err_type.UNKNOWN_VAR; }
                                    }
                                    else if (FUN_variant[Index][pair + 1] == "@Q")
                                    {
                                        for (int find_fun = 0; find_fun < Q_name.Count; find_fun++)
                                        {
                                            if (Q_name[find_fun] == FUN_variant[Index][pair])
                                            {
                                                fun_var = find_fun; break;
                                            }
                                        }
                                        for (int find_fun = 0; find_fun < Q_name.Count; find_fun++)
                                        {
                                            if (Q_name[find_fun] == variant[pair])
                                            {
                                                inp_var = find_fun; break;
                                            }
                                        }
                                        if (fun_var != -1 && inp_var != -1)
                                            Q_value[inp_var] = Q_value[fun_var];
                                        else { err_message = Err_type.UNKNOWN_VAR; }
                                    }
                                    else { err_message = Err_type.SYNTEX_ERR; }
                                }
                            }
                            else { err_message = Err_type.SYNTEX_ERR; }
                        else { err_message = Err_type.UNKNOWN_VAR; }
                    }
                }
                else if (str_now == "Find")//引用
                {
                    int Index;
                    ptr_now = r;
                    string name = string_switch[ptr_now + 1],
                        start_of_var = string_switch[ptr_now + 2];
                    Index = FUN_name.IndexOf(name);

                    if (in_tokens) IndexOf_name = Index;
                    else if (in_function) IndexOf_name2 = Index;

                    if (Index != -1 && start_of_var == "(")
                    {
                        int var_end, var_test = -1;
                        for (var_end = ptr_now; var_end < code_length; var_end++)
                        {
                            if (string_switch[var_end] == ")")
                            {
                                var_test = var_end;
                                break;
                            }
                        }
                        if (var_test != -1)
                        {
                            List<String> variant = new List<String>();
                            for (int var_beg = ptr_now + 3; var_beg < var_end; var_beg++)
                            {
                                if (string_switch[var_beg] != "")
                                    variant.Add(string_switch[var_beg]);
                            }
                            int variant_Count = variant.Count;
                            if (FUN_variant[Index].Count == variant_Count && variant_Count % 2 == 0)
                            {
                                for (int pair = 0; pair < variant_Count / 2; pair++)
                                {
                                    int fun_var = -1, inp_var = -1;
                                    if (FUN_variant[Index][2 * pair + 1] == "@I")
                                    {
                                        for (int find_fun = 0; find_fun < I_name.Count; find_fun++)
                                        {
                                            if (I_name[find_fun] == FUN_variant[Index][2 * pair])
                                            {
                                                fun_var = find_fun; break;
                                            }
                                        }
                                        for (int find_fun = 0; find_fun < I_name.Count; find_fun++)
                                        {
                                            if (I_name[find_fun] == variant[2 * pair])
                                            {
                                                inp_var = find_fun; break;
                                            }
                                        }
                                        if (fun_var != -1 && inp_var != -1)
                                            I_value[fun_var] = I_value[inp_var];
                                        else { err_message = Err_type.UNKNOWN_VAR; }
                                    }
                                    else if (FUN_variant[Index][2 * pair + 1] == "@F")
                                    {
                                        for (int find_fun = 0; find_fun < F_name.Count; find_fun++)
                                        {
                                            if (F_name[find_fun] == FUN_variant[Index][2 * pair])
                                            {
                                                fun_var = find_fun; break;
                                            }
                                        }
                                        for (int find_fun = 0; find_fun < F_name.Count; find_fun++)
                                        {
                                            if (F_name[find_fun] == variant[2 * pair])
                                            {
                                                inp_var = find_fun; break;
                                            }
                                        }
                                        if (fun_var != -1 && inp_var != -1)
                                            F_value[fun_var] = F_value[inp_var];
                                        else { err_message = Err_type.UNKNOWN_VAR; }
                                    }
                                    else if (FUN_variant[Index][pair + 1] == "@C")
                                    {
                                        for (int find_fun = 0; find_fun < C_name.Count; find_fun++)
                                        {
                                            if (C_name[find_fun] == FUN_variant[Index][2 * pair])
                                            {
                                                fun_var = find_fun; break;
                                            }
                                        }
                                        for (int find_fun = 0; find_fun < C_name.Count; find_fun++)
                                        {
                                            if (C_name[find_fun] == variant[2 * pair])
                                            {
                                                inp_var = find_fun; break;
                                            }
                                        }
                                        if (fun_var != -1 && inp_var != -1)
                                            C_value[fun_var] = C_value[inp_var];
                                        else { err_message = Err_type.UNKNOWN_VAR; }
                                    }
                                    else if (FUN_variant[Index][2 * pair + 1] == "@Q")
                                    {
                                        for (int find_fun = 0; find_fun < Q_name.Count; find_fun++)
                                        {
                                            if (Q_name[find_fun] == FUN_variant[Index][2 * pair])
                                            {
                                                fun_var = find_fun; break;
                                            }
                                        }
                                        for (int find_fun = 0; find_fun < Q_name.Count; find_fun++)
                                        {
                                            if (Q_name[find_fun] == variant[2 * pair])
                                            {
                                                inp_var = find_fun; break;
                                            }
                                        }
                                        if (fun_var != -1 && inp_var != -1)
                                            Q_value[fun_var] = Q_value[inp_var];
                                        else { err_message = Err_type.UNKNOWN_VAR; }
                                    }
                                    else { err_message = Err_type.SYNTEX_ERR; }
                                }
                                r = 0;
                                if (in_tokens)
                                {
                                    in_tokens = false;
                                    in_function = true;
                                }
                                else if (in_function)
                                {
                                    in_function = false;
                                    in_function2 = true;
                                }
                                else if (in_function2)
                                {
                                }
                            }
                            else { err_message = Err_type.SYNTEX_ERR; }
                            if (in_function == true)
                            {
                                fun_escape = var_end;
                                Abstra = 0;
                            }
                            else if (in_function2 == true)
                            {
                                fun_escape2 = var_end;
                                Abstra = 0;
                            }
                        }
                        else { err_message = Err_type.SYNTEX_ERR; }
                    }
                }
                #endregion
                #region 函式庫
                else if (str_now == "Claim")
                {
                    ptr_now = r;
                    string lib_name = string_switch[ptr_now + 1] + ".ss";
                    string lib_path = Path.GetFullPath(lib_name);
                    System.IO.StreamReader ss_file = null;
                    if (File.Exists(@lib_path))
                    {
                        LIB_name.Add(lib_name);
                        ss_file = new System.IO.StreamReader(@lib_path);
                        string ss_line = "", line_temp;
                        while ((line_temp = ss_file.ReadLine()) != null)
                            ss_line += line_temp;
                        string[] ss_tokens = ss_line.Split(' ', '\n');
                        int ss_token_length = ss_tokens.Length;
                        List<String> ss_text = new List<String>();
                        for (int input = 0; input < ss_token_length; input++)
                        {
                            if (ss_tokens[input] != "")
                                ss_text.Add(ss_tokens[input]);
                        }
                        LIB_value.Add(ss_text);
                        r = 0; code_length = ss_text.Count; lib_escape = ptr_now;
                        in_library = true;
                    }
                }
                else if (str_now == "QED")
                {
                    r = lib_escape + 1; code_length = tokens.Length;
                    in_library = false;
                }
                #endregion
                //錯誤訊息輸出
                if (err_message != Err_type.DEFAULT)
                    err_output += "LIBRARY SECTION "+ r.ToString() + " " + str_now + " " + err_message.ToString() + " \r\n";
            }
            ErrBox.Text = "";
            if (err_output != "")
                ErrBox.Text = err_output;
            else if (output != "")
            {
                AllocConsole();
                Console.WriteLine(output);
            }

        }

        private void finish_compile(object sender, EventArgs e)
        {
            AttachConsole(-1);
            FreeConsole();
        }

        private bool Complex_TryParse(string input, out Complex result )
        {
            result.R = 0.0;
            result.I = 0.0;
            bool is_com = false;
            double test_R = 0.0, test_I = 0.0;
            string[] tokens = input.Split('\\', '|', '/');
            if (tokens.Length == 4)
            {
                bool result_R = double.TryParse(tokens[1], out test_R),
                     result_I = double.TryParse(tokens[2], out test_I);
                if (result_R && result_I)
                {
                    result.R = test_R;
                    result.I = test_I;
                    is_com = true;
                }
            }
            return is_com;
        }
        private bool Quaternion_TryParse(string input, out Quaternion result)
        {
            result.R = 0.0;
            result.I = 0.0;
            result.J = 0.0;
            result.K = 0.0;
            bool is_com = false;
            double test_R = 0.0, test_I = 0.0, test_J = 0.0, test_K = 0.0;
            string[] tokens = input.Split('\\', '|', '/');
            if (tokens.Length == 6)
            {
                bool result_R = double.TryParse(tokens[1], out test_R),
                     result_I = double.TryParse(tokens[2], out test_I),
                     result_J = double.TryParse(tokens[3], out test_J),
                     result_K = double.TryParse(tokens[4], out test_K);
                if (result_R && result_I && result_J && result_K)
                {
                    result.R = test_R;
                    result.I = test_I;
                    result.J = test_J;
                    result.K = test_K;
                    is_com = true;
                }
            }
            return is_com;
        }

        private void execute(object sender, EventArgs e)
        {
            if (SSR_name == "")
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Title = "Folder";
                dialog.Filter = "SSR(*.ssr)|*.ssr";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SSR_name = dialog.FileName;
                }
            }
            if(SSR_name != "")
            {
                StreamWriter sw = new StreamWriter(SSR_name);
                String string_to_write = " ";

                int ptr_now = 0;
                string[] raw_tokens = CodeBox.Text.Split(' ', '\n');
                int raw = raw_tokens.Length;
                string[] tokens = new string[raw]; int not_null = 0;
                for (int r = 0; r < raw; r++) { if (raw_tokens[r] != "") { tokens[not_null] = raw_tokens[r]; not_null++; } }
                int code_length = tokens.Length;
                for (int r = 0; r < code_length; r++)
                {

                    string str_now = tokens[r];
                    if (str_now != "Claim" && str_now != "QED")
                    {
                        string_to_write += str_now + " ";
                    }
                    else if (str_now == "Claim")
                    {
                        ptr_now = r;
                        string lib_name = tokens[ptr_now + 1] + ".ss";
                        string lib_path = Path.GetFullPath(lib_name);
                        StreamReader ss_file = null;
                        if (File.Exists(@lib_path))
                        {
                            ss_file = new StreamReader(@lib_path);
                            string ss_line = "", line_temp;
                            while ((line_temp = ss_file.ReadLine()) != null)
                                ss_line += line_temp;
                            string[] ss_tokens = ss_line.Split(' ', '\n');
                            int ss_token_length = ss_tokens.Length;
                            List<String> ss_text = new List<String>();
                            for (int input = 0; input < ss_token_length; input++)
                            {
                                if (ss_tokens[input] != "" && ss_tokens[input] != "QED") string_to_write += ss_tokens[input] + " ";
                            }
                            r ++; 
                        }
                    }


                }

                sw.WriteLine(string_to_write);
                sw.Close();
            }
        }

        private void open_file(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Folder";
            dialog.Filter = "SS(*.ss)|*.ss";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SS_name = dialog.FileName;
            }
            if (SS_name != "")
            {
                StreamReader ss_file = new StreamReader(SS_name);
                string ss_line = "", line_temp;

                while ((line_temp = ss_file.ReadLine()) != null)
                    ss_line += line_temp + "\n";
                CodeBox.Clear();
                CodeBox.AppendText(ss_line);
            }

        }

        private void save_file(object sender, EventArgs e)
        {

        }
    }
}
