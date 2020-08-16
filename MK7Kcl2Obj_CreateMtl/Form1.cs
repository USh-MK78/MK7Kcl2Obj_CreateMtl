using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace MK7Kcl2Obj_CreateMtl
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog()
            {
                Title = "OBJファイルを開く",
                InitialDirectory = @"C:\Users\User\Desktop",
                Filter = "obj file|*.obj"
            };

            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;

            System.IO.StreamReader sr1 = new System.IO.StreamReader(openFileDialog1.FileName);

            string Ary1;
            //1行ずつ読み込み
            while(sr1.Peek() > -1)
            {
                Ary1 = sr1.ReadLine();
                listBox1.Items.Add(Ary1);
            }
            sr1.Close();

            //listBoxのアイテムを全て取得して配列に格納
            string[] AllItem = this.listBox1.Items.Cast<string>().ToArray();

            //listBoxのアイテムを全てカウント
            int CountList = listBox1.Items.Count;

            //Listを生成
            System.Collections.Generic.List<string> df = new System.Collections.Generic.List<string>(AllItem);
            //usemtlの要素を取得する
            IEnumerable<string> r = df.TakeWhile(value => value.StartsWith("usemtl"));
            //要素の文字列の始まりに[fの文字がある場合]はその要素を削除
            df.RemoveAll(value => value.StartsWith("f "));
            //要素の文字列の始まりに[vの文字がある場合]はその要素を削除
            df.RemoveAll(value => value.StartsWith("v "));
            df.RemoveAll(value => value.StartsWith("#"));
            //要素の重複を禁止する
            df.Distinct().ToList();

            //一度listBoxを空にする
            listBox1.Items.Clear();
            //Listの内容をlistBoxに表示
            listBox1.Items.AddRange(df.ToArray());

            //listBoxのアイテムを全て取得して配列に格納
            string[] Remove2Duplicate = this.listBox1.Items.Cast<string>().ToArray();

            //一意の要素を一時的に格納しておくコレクション
            System.Collections.ArrayList R2D_AryList = new System.Collections.ArrayList(Remove2Duplicate.Length);

            //元になる配列の列挙
            foreach (string i in Remove2Duplicate)
            {
                //コレクション内に存在していなければ追加する
                if (!R2D_AryList.Contains(i))
                {
                    R2D_AryList.Add(i);
                }
            }

            //string配列に変換
            string[] R2D_ResultAry = (string[])R2D_AryList.ToArray(typeof(string));

            //既にあるItemを全てクリア(削除)する
            listBox1.Items.Clear();

            //重複を削除した配列をlistBoxに表示
            foreach (string mtllist_Fix in R2D_ResultAry)
            {
                listBox1.Items.Add(mtllist_Fix);
            }

            //空のlistを削除
            listBox1.Items.RemoveAt(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog()
            {
                Title = "mtlファイルを作成",
                InitialDirectory = @"C:\Users\User\Desktop",
                Filter = "mtl file|*.mtl"
            };

            if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;

            //listboxに存在する複数のアイテムを取得
            string str = "\r\n";
            ListBox.ObjectCollection list1 = listBox1.Items;
            foreach (object liststr in list1)
            {
                str = str + liststr.ToString().Replace("usemtl", "newmtl") +"\r\n" +  "Ns 96.078431\r\nKa 1.000000 1.000000 1.000000\r\nKd 0.640000 0.640000 0.640000\r\n" +
                    "Ks 0.500000 0.500000 0.500000\r\nKe 0.000000 0.000000 0.000000\r\nNi 1.000000\r\nd 1.000000\r\nillum 2\r\n\r\n";
            }
            //newmtl XXXX
            //Ns 96.078431
            //Ka 1.000000 1.000000 1.000000
            //Kd 0.640000 0.640000 0.640000
            //Ks 0.500000 0.500000 0.500000
            //Ke 0.000000 0.000000 0.000000
            //Ni 1.000000
            //d 1.000000
            //illum 2
            File.WriteAllText(saveFileDialog1.FileName, str);

            OpenFileDialog openFileDialog2 = new OpenFileDialog()
            {
                Title = "OBJファイルを開く",
                InitialDirectory = @"C:\Users\User\Desktop",
                Filter = "obj file|*.obj"
            };

            if (openFileDialog2.ShowDialog() != DialogResult.OK) return;

            //objファイルの読み込み
            //objファイルをテキストとして全て読み込み
            string objstr = System.IO.File.ReadAllText(openFileDialog2.FileName);
            //mtlファイルのパスからファイル名のみ取得
            string mtlpath = Path.GetFileName(saveFileDialog1.FileName);
            //先頭の1行をReplaceで置き換える
            string addmtl = objstr.Replace("# Created by Every File Explorer", "# Created by Every File Explorer\r\nmtllib " + mtlpath);
            //mtlファイルと同じ場所に書き換えたobjファイルを保存するため、mtlファイルの保存先パスからディレクトリのみ取得
            string MTLPath2Dir = System.IO.Path.GetDirectoryName(saveFileDialog1.FileName);
            //objファイルのパスからファイル名のみ取得
            string OBJPath2FileName = Path.GetFileName(openFileDialog2.FileName);
            //パスを結合して保存
            System.IO.File.WriteAllText(MTLPath2Dir + "\\"+ OBJPath2FileName + "_Addmtl.obj", addmtl);
        }
    }
}
