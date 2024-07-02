using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestSLC2LDF
{
    public static class SLC2LDF
    {
        private const ushort EN_Bit = 0x8000;
        private const ushort TT_Bit = 0x4000;
        private const ushort DN_Bit = 0x2000;

        public static string[] ReadSLC(string path)
        {
            return File.ReadAllLines(path);
        }

        public static string[] GetTextRang(string[] FileText)
        {
            List<string> text = new List<string>();
            foreach (string st in FileText)
            {
                if(st.Contains("SOR") && st.Contains("EOR")) text.Add(st.Replace("SOR","").Replace("EOR ", ""));
            }
            return text.ToArray();
        }

        public static string[] GetTextRang(string path)
        {
            string[] FileText = File.ReadAllLines(path);
            List<string> text = new List<string>();
            foreach (string st in FileText)
            {
                if (st.Contains("SOR") && st.Contains("EOR")) text.Add(st.Replace("SOR", "").Replace("EOR ", ""));
            }
            return text.ToArray();
        }

        public static Dictionary<string, ushort[]> GetData(string[] FileText)
        {
            Dictionary<string, ushort[]> Data = new Dictionary<string, ushort[]>();
            for (int i = 0; i < FileText.Length; i++)
            {
                string st = FileText[i];
                if (st.Contains("DATA"))
                {
                    string name = st.Replace("DATA ", "").Trim().Split(':')[0];
                    string data = "";
                    i++;

                    while (true)
                    {
                        st = FileText[i];
                        if (st != "")
                        {
                            data += st;
                            i++;
                        }
                        else
                            break;
                    }

                    if (data != "")
                    {
                        string[] str_buf = data.Trim().Split(' ').Where(x => x != "").ToArray();

                        if (name == "T4")
                        {
                            List<ushort> con = new List<ushort>();
                            List<ushort> pres = new List<ushort>();
                            List<ushort> acc = new List<ushort>();
                            for (int j = 0; j < str_buf.Length; j++)
                            {
                                ushort buf = (ushort)Convert.ToInt16(str_buf[j], 16);
                                ushort value = 0;
                                if ((buf & EN_Bit) == EN_Bit) value |= 1;
                                if ((buf & DN_Bit) == DN_Bit) value |= 2;
                                if ((buf & TT_Bit) == TT_Bit) value |= 4;

                                if((buf&256) == 256) value |= 16;
                                else if((buf&512) == 512) value |= 32;

                                con.Add(value);
                                pres.Add((ushort)short.Parse(str_buf[j + 1]));
                                acc.Add((ushort)short.Parse(str_buf[j + 2]));
                                j += 2;
                            }
                            Data.Add("T4", pres.ToArray());
                            Data.Add("T4_c", acc.ToArray());
                            Data.Add("Timer_control", con.ToArray());
                        }
                        else if (name != "F8" && name != "HSC0")
                        {

                            List<ushort> data_to_out = new List<ushort>();
                            foreach (string s in str_buf)
                            {
                                if (s.Contains("0X"))
                                    data_to_out.Add((ushort)Convert.ToInt16(s, 16));
                                else
                                    data_to_out.Add((ushort)short.Parse(s));
                            }

                            Data.Add(name, data_to_out.ToArray());
                        }
                    }
                }
            }
            return Data;
        }

        public static Dictionary<string, ushort[]> GetData(string path)
        {
            string[] FileText = File.ReadAllLines(path);
            Dictionary<string, ushort[]> Data = new Dictionary<string, ushort[]>();
            for (int i = 0; i < FileText.Length; i++)
            {
                string st = FileText[i];
                if (st.Contains("DATA"))
                {
                    string name = st.Replace("DATA ", "").Trim().Split(':')[0];
                    string data = "";
                    i++;

                    while (true)
                    {
                        st = FileText[i];
                        if (st != "")
                        {
                            data += st;
                            i++;
                        }
                        else
                            break;
                    }

                    if (data != "")
                    {
                        string[] str_buf = data.Trim().Split(' ').Where(x => x != "").ToArray();

                        if (name == "T4")
                        {
                            List<ushort> con = new List<ushort>();
                            List<ushort> pres = new List<ushort>();
                            List<ushort> acc = new List<ushort>();
                            for (int j = 0; j < str_buf.Length; j++)
                            {
                                ushort buf = (ushort)Convert.ToInt16(str_buf[j], 16);
                                ushort value = 0;
                                if ((buf & EN_Bit) == EN_Bit) value |= 1;
                                if ((buf & DN_Bit) == DN_Bit) value |= 2;
                                if ((buf & TT_Bit) == TT_Bit) value |= 4;

                                if ((buf & 256) == 256) value |= 16;
                                else if ((buf & 512) == 512) value |= 32;

                                con.Add(value);
                                pres.Add((ushort)short.Parse(str_buf[j + 1]));
                                acc.Add((ushort)short.Parse(str_buf[j + 2]));
                                j += 2;
                            }
                            Data.Add("T4", pres.ToArray());
                            Data.Add("T4_c", acc.ToArray());
                            Data.Add("Timer_control", con.ToArray());
                        }
                        else if (name != "F8" && name != "HSC0")
                        {

                            List<ushort> data_to_out = new List<ushort>();
                            foreach (string s in str_buf)
                            {
                                if (s.Contains("0X"))
                                    data_to_out.Add((ushort)Convert.ToInt16(s, 16));
                                else
                                    data_to_out.Add((ushort)short.Parse(s));
                            }

                            Data.Add(name, data_to_out.ToArray());
                        }
                    }
                }
            }
            return Data;
        }
    }

    public static class LDF2SLC
    {
        private const ushort EN_Bit = 0x8000;
        private const ushort TT_Bit = 0x4000;
        private const ushort DN_Bit = 0x2000;

        private static readonly string header = "START Bul.1763    % Bul.1763     MicroLogix 1400 Series B %\r\n\r\nSLOT 0 Bul.1763    % Bul.1766     MicroLogix 1400 Series B %  SCAN_IN 8 SCAN_OUT 6  G_FILE 17\r\n     526 0 0 0 0 0 0 0 0 0 0 0 0 0 0 255 255 IOCARD 4096 0 8 0 0 6 0 0 0 2 0 0\r\n\r\n\r\nPROJECT \"SCAN_11\"\r\n     2 \"MAIN\"\r\n     3 \"INIT\"\r\n     4 \"CONTROL\"\r\n\r\nLADDER 2\r\n\r\nLADDER 3\r\n\r\nLADDER 4";

        public static string CreateData(string[] _Rangs, Dictionary<string, ushort[]> _Data)
        {
            string[] rangs = _Rangs;
            foreach (string s in _Rangs)
            {
                if (!s.Contains("SOR") && !s.Contains("EOR"))
                {
                    rangs = EditRang(_Rangs);
                    break;
                }
            }
            string write = header + "\n" + string.Join("\n", rangs) + "\n\n";

            string[] first_keys = SortKey(_Data.Keys.ToArray()).Where(x => x!=null).ToArray();
            foreach (string key in first_keys)
            {
                string buf = "";
                if (key == "T4")
                {
                    for (int i = 0; i < _Data[key].Length; i++)
                    {
                        int d = 0;
                        if ((_Data["Timer_control"][i]&1)==1) d |= EN_Bit;
                        if ((_Data["Timer_control"][i]&2)==2) d |= DN_Bit;
                        if ((_Data["Timer_control"][i]&4)==4) d |= TT_Bit;

                        if ((_Data["Timer_control"][i] & 16) == 16) d |= 256;
                        else if ((_Data["Timer_control"][i] & 32) == 32) d |= 512;

                        buf += "0X" + new string('0', 4 - d.ToString("X").Length) + d.ToString("X") + " " 
                            + new string(' ', 6 - ((short)_Data["T4"][i]).ToString().Length) + ((short)_Data["T4"][i]).ToString() + " "
                            + new string(' ', 6 - ((short)_Data["T4_c"][i]).ToString().Length) + ((short)_Data["T4_c"][i]).ToString() + " \n";
                    }
                    write += $"DATA {key}:0\n" + buf + "\n";
                }
                else
                {
                    byte count = 0;
                    for (int i = 0; i < _Data[key].Length; i++)
                    {
                        string value = ((short)_Data[key][i]).ToString();
                        if (count == 10)
                        {
                            buf += "\n";
                            count = 0;
                        }
                        buf += new string(' ', 6-value.Length)+value+" ";
                        count++;
                    }
                    write += $"DATA {key}:0\n" + buf + "\n";
                }
                write += "\n";
            }

            return write;
        }

        private static string[] SortKey(string[] _keys)
        {
            string[] arr_out = new string[_keys.Length-2];

            string[] key = _keys.Where(x => x!="T4_c" && x!="Timer_control").Where(x => x.Length >1).ToArray();

            string[] id = key.Where(x => byte.TryParse(x[1].ToString(), out byte u)).Select(x => new Regex(@"[A-Z]").Replace(x, "")).ToArray();
            int[] buf = id.Select(x => int.Parse(x)).ToArray();
            Array.Sort(buf);
            id = buf.Select(x => x.ToString()).ToArray();
            for (int i = 0; i < id.Length; i++)
            {
                for (int j = 0; j < key.Length; j++)
                {
                    if (id[i] == new Regex(@"[A-Z]").Replace(key[j], "")) arr_out[i] = key[j];
                }
            }
            return arr_out;
        }

        private static string[] EditRang(string[] _Rangs)
        {
            string[] rangs = new string[_Rangs.Length];
            for(int i = 0;i < _Rangs.Length;i++)
            {
                rangs[i] = "SOR " + new Regex(@"\ASOR|EOR\z").Replace(_Rangs[i], "").Trim() + " EOR";
            }
            return rangs;
        }
    }
}
