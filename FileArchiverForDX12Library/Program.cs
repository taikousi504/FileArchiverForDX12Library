using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileArchiverForDX12Library
{
    class Program
    {
        static void Main(string[] args)
        {
            //D&Dされたパスリスト
            List<string> paths = new List<string>();
            //パスリストに含まれる全てのファイルリスト
            Dictionary<string, List<string>> files = new Dictionary<string, List<string>>();
            //出力名
            string dstName;
            //ファイルサイズコンテナ
            List<long> dataSize = new List<long>();

            //実行ファイルに直接D&Dされていなかった場合
            if (args.Length == 0)
            {
                //パスを文字列として受け取る
                Console.WriteLine("アーカイブしたいフォルダをドラッグ&ドロップして実行してください。");
                Console.WriteLine("何かキーを押すと終了します...");
                Console.ReadKey();
                return;
            }
            else
            {
                //全引数をリストに追加
                foreach (var v in args)
                {
                    paths.Add(v);
                }

                //出力名を指定させる
                Console.Write("出力名を入力してください。\n(拡張子記述は不要です)\nName:");
                dstName = Console.ReadLine();
                dstName += ".ark";
            }

            //フォルダに格納されているすべてのファイルを列挙
            foreach (var v in paths)
            {
                //リスト生成
                files.Add(v, new List<string>());

                //全ファイル取得
                string[] tmp = Directory.GetFiles(v, "*", SearchOption.AllDirectories);

                //ファイル名をリストに追加
                foreach(var v2 in tmp)
                {
                    //元ディレクトリより前のディレクトリ情報は削る
                    string parentDir = v.Substring(v.LastIndexOf("\\") + 1);
                    string path = v2.Substring(v2.IndexOf(parentDir));

                    files[v].Add(path);
                }
            }

            //ファイル出力準備
            string dstDir = args[0].Substring(0, args[0].LastIndexOf("\\"));
            BinaryWriter sw = new BinaryWriter(new FileStream(dstDir + "\\" + dstName, FileMode.OpenOrCreate));

            //ファイルを一つ一つ書き出し
            List<byte> output = new List<byte>();
            string pwd = Environment.CurrentDirectory;

            //データ部
            foreach (var v1 in files)
            {
                //カレントディレクトリを変更
                Environment.CurrentDirectory = v1.Key.Substring(0, v1.Key.LastIndexOf("\\"));

                foreach (var v2 in v1.Value)
                {
                    Console.Write(v2 + "を書き込みます...");

                    //まずは全データ結合
                    BinaryReader br = new BinaryReader(new FileStream(v2, FileMode.Open));

                    //データ追加
                    for (int i = 0; i < br.BaseStream.Length; i++)
                    {
                        output.Add(br.ReadByte());
                    }

                    dataSize.Add(br.BaseStream.Length);

                    br.Close();

                    Console.WriteLine("完了");
                }
            }

            string header = "";
            int index = 0;
            long startPos = 0;
            //ヘッダー部
            foreach (var v1 in files)
            {
                //カレントディレクトリを変更
                Environment.CurrentDirectory = v1.Key.Substring(0, v1.Key.LastIndexOf("\\"));

                foreach (var v2 in v1.Value)
                {
                    //ファイル名
                    header += "\"" + v2.Replace("\\", "/") + "\"";
                    //開始位置とデータサイズ挿入
                    header += startPos + "," + dataSize[index];

                    startPos += dataSize[index];
                    index++;
                }
            }

            //さいごにヘッダーサイズと識別子を挿入
            const int IDENTIFIER_SIZE = 16;
            string headerSize = (Encoding.UTF8.GetByteCount(header) + IDENTIFIER_SIZE).ToString();

            while(headerSize.Length < 12)
            {
                headerSize = "0" + headerSize;
            }

            header = "ARCK" + headerSize + header;


            Console.Write(dstDir + "にファイルを出力...");

            //カレントディレクトリを変更
            Environment.CurrentDirectory = pwd;

            //書き出し
            sw.Write(header.ToCharArray());
            sw.Write(output.ToArray());

            sw.Close();
            paths.Clear();
            files.Clear();
            dataSize.Clear();

            Console.WriteLine("完了");

            Console.WriteLine("\nすべての処理が完了しました。\n何かキーを押すと終了します...");
            Console.ReadKey();
        }
    }
}
