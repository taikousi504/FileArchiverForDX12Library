using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileArciverForDX12Library
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> paths = new List<string>();
            List<string> files = new List<string>();
            string dstName;

            //実行ファイルに直接D&Dされていなかった場合
            if (args.Length == 0)
            {
                //パスを文字列として受け取る
                Console.WriteLine("アーカイブしたいフォルダをドラッグ&ドロップして実行してください。");
                Console.WriteLine("何かキーを押すと終了します...");
                Console.ReadLine();
                return;
            }
            else
            {
                //全引数をリストに追加
                foreach (var v in args)
                {
                    paths.Add(v);
                }

                //引数が1つなら出力名はフォルダ名.arkにする
                if (args.Length == 1)
                {
                    string parentDir = args[0].Substring(args[0].LastIndexOf("\\") + 1);
                    dstName = parentDir + ".ark";
                }
                //引数複数なら名前を指定させる
                else
                {
                    Console.Write("出力名を入力してください。\n(拡張子記述は不要です)\nName:");
                    dstName = Console.ReadLine();
                    dstName += ".ark";
                }
            }

            //フォルダに格納されているすべてのファイルを列挙
            foreach (var v in paths)
            {
                //全ファイル取得
                string[] tmp = Directory.GetFiles(v, "*", SearchOption.AllDirectories);

                //ファイル名をリストに追加
                foreach(var v2 in tmp)
                {
                    //元ディレクトリより前のディレクトリ情報は削る
                    string parentDir = v.Substring(v.LastIndexOf("\\") + 1);
                    string path = v2.Substring(v2.IndexOf(parentDir));

                    files.Add(path);
                }
            }

            //ファイル出力準備
            string dstDir = args[0].Substring(0, args[0].LastIndexOf("\\"));
            StreamWriter sw = new StreamWriter(dstDir + "\\" + dstName);

            //テスト出力
            foreach (var v in files)
            {
                Console.WriteLine(v);
                sw.WriteLine(v);
            }

            sw.Close();

            Console.WriteLine("何かキーを押すと終了します...");
            Console.ReadLine();
        }
    }
}
