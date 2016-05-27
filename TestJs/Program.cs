using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestJs
{
	class Program
	{
		private static Dictionary<string,SwordModel> dic = new Dictionary<string, SwordModel>();
		private static Dictionary<string,SwordModel> orderedDic = new Dictionary<string, SwordModel>();
		
		public class SwordModel
		{
			public string name;
			public int id;
			public int rarity;
			public int type;
			public int group;
			public int equip;
			public int area;
			public int upgrade;
		}
		
		private static string FormatJson(string str)
		{
			JsonSerializer serializer = new JsonSerializer();
			TextReader tr = new StringReader(str);
			JsonTextReader jtr = new JsonTextReader(tr);
			object obj = serializer.Deserialize(jtr);
			if (obj != null)
			{
				StringWriter textWriter = new StringWriter();
				JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
				{
					Formatting = Formatting.Indented,
					Indentation = 4,
					IndentChar = ' '
				};
				serializer.Serialize(jsonWriter, obj);
				return textWriter.ToString();
			}
			else
			{
				return str;
			}
		}
		
		private static int FindLast(string key,string source,int startIndex = 0)
		{
			int index = source.IndexOf(key,startIndex);
			return index + key.Length;
		}
		
		public static void Main(string[] args)
		{
			StreamReader sr = new StreamReader(Environment.CurrentDirectory + @"\define.js");
			string textData = sr.ReadToEnd();
			int head = FindLast("exports.tohken.define.tohkens = ",textData);
			int tail = FindLast("};",textData,head) - 1;
			string data2 = textData.Substring(head,tail - head).Trim();
			//JObject jo = JObject.Parse(data2);
			dic = JsonConvert.DeserializeObject<Dictionary<string,SwordModel>>(data2);
			
			dic.Add("0",new SwordModel(){ name = "SANIWA",id = 0,rarity = 5,type = 900,group = 900,equip = 3,area = 1,upgrade = 25});
			
			foreach(KeyValuePair<string,SwordModel> item in dic.OrderBy(e => int.Parse(e.Key)))
			{
				orderedDic.Add(item.Key,item.Value);
			}
			
			string newJson = FormatJson(JsonConvert.SerializeObject(orderedDic));
			textData = textData.Remove(head,tail - head);
			textData = textData.Insert(head,newJson);
			
			Console.WriteLine(textData);
			Console.ReadKey();
		}
	}
}