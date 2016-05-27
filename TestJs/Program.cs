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
		private static Dictionary<string,SwordModel> dic = new Dictionary<string, SwordModel>();			//Sword list.
		private static Dictionary<string,SwordModel> orderedDic = new Dictionary<string, SwordModel>();		//Sorted sword list.
		
		private class SwordModel																			//Json model(used to deserialize json).								
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
		
		private static string FormatJson(string str)														//Indent a json.
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
		
		private static int Find(string key,string source,int startIndex = 0)								//Find string and return the index of last char.
		{
			int index = source.IndexOf(key,startIndex);
			return index + key.Length;
		}
		
		public static void Main(string[] args)
		{
			StreamReader sr = new StreamReader(Environment.CurrentDirectory + @"\define.js");
			string textData = sr.ReadToEnd();
			sr.Close();
			sr.Dispose();
			
			int head = Find("exports.tohken.define.tohkens = ",textData);									//Start index.
			int tail = Find("};",textData,head) - 1;														//End index.
			string data2 = textData.Substring(head,tail - head).Trim();										//Js between the two indexs.These text can be parsed as a json.
			dic = JsonConvert.DeserializeObject<Dictionary<string,SwordModel>>(data2);						//Parse the text and deserialize each item into the list.
			dic.Add("0",new SwordModel(){ name = "SANIWA",id = 0,rarity = 5,type = 900,group = 900,equip = 3,area = 1,upgrade = 25});
																											//Not a valid sword.Just for test.
			
			foreach(KeyValuePair<string,SwordModel> item in dic.OrderBy(e => int.Parse(e.Key)))				//Sort.
			{
				orderedDic.Add(item.Key,item.Value);
			}
			
			string newJson = FormatJson(JsonConvert.SerializeObject(orderedDic));
			textData = textData.Remove(head,tail - head);													//Remove old js.
			textData = textData.Insert(head,newJson);														//Insert new js.
			textData = textData.Replace("\r","");															//Just keep LF.
			
			StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + @"\_define.js");				//Write file(for test,write to another file).
			sw.BaseStream.SetLength(0);
			sw.Write(textData);
			sw.Close();
			sw.Dispose();
		}
	}
}