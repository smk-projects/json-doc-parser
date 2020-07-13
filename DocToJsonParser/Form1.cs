using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Dynamic;
using Newtonsoft.Json;

namespace DocToJsonParser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                this.txtOutput.ForeColor = System.Drawing.SystemColors.WindowText;
                this.txtOutput.Text = this.ConvertToJsonString(this.txtInput.Text.Trim());
            }
            catch (JsonTagParseException ex)
            {
                this.txtOutput.ForeColor = System.Drawing.Color.Red;
                this.txtOutput.Text = ex.Message;
            }
                
        }
        /// <summary>
        /// Convert Doc Text To Json String
        /// </summary>
        private string ConvertToJsonString(string docText)
        {
            var jsonLines = docText.Split('\n');
            var lineNumber = 0;

            IDictionary<string, object> jsonObject = new Dictionary<string, object>();
            // List<IDictionary<string, object>> jsonObjectTree = new List<IDictionary<string, object>>();
            List<object> jsonObjectTree = new List<object>();
            foreach (var jsonLine in jsonLines)
            {
                lineNumber++;
                if (string.IsNullOrEmpty(jsonLine.Trim()))
                    continue;

                var jsonTag = GetJsonTagByLineText(lineNumber, jsonLine);
                JsonTag nextJsonTag = null;
                if (lineNumber < jsonLines.Length)
                    nextJsonTag = GetJsonTagByLineText(lineNumber, jsonLines[lineNumber]);

                if (jsonObjectTree.Count == 0)
                    jsonObjectTree.Add(jsonObject);

                var isArray = jsonTag.DataType.EndsWith("[]");
                var dataTypeMatches = Regex.Matches(jsonTag.DataType, @"^(string|number|boolean|object)(\[\])?$");
                if (dataTypeMatches.Count == 0)
                    throw new JsonTagParseException(lineNumber, $"Invalid DataType: {jsonTag.DataType}");

                var dataType = dataTypeMatches[0].Value;

                var nextLevelDiff = (nextJsonTag?.Level ?? jsonTag.Level) - jsonTag.Level;

                IDictionary<string, object> currJsonObject;
                if (jsonObjectTree.Last() is IDictionary<string, object>)
                    currJsonObject = jsonObjectTree.Last() as IDictionary<string, object>;
                else
                    currJsonObject = (jsonObjectTree.Last() as List<Dictionary<string, object>>).First();

                if (nextLevelDiff <= 0)
                {
                    if (!isArray)
                        currJsonObject.Add(jsonTag.TagName, GetJsonTagDescription(jsonTag));
                    else
                        currJsonObject.Add(jsonTag.TagName, new string[] { GetJsonTagDescription(jsonTag), "..." });

                    if (nextLevelDiff < 0)
                    {
                        if (Math.Abs(nextLevelDiff) >= jsonObjectTree.Count())
                            throw new JsonTagParseException(lineNumber, $"Next Level ({nextLevelDiff}) Greater Then Json Tree Count ({jsonObjectTree.Count()}) ");

                        for (var i = 0; i < Math.Abs(nextLevelDiff); i++)
                            jsonObjectTree.Remove(jsonObjectTree.Last());
                    }
                    continue;
                }

                if (nextLevelDiff > 0 && nextLevelDiff != 1)
                    throw new JsonTagParseException(lineNumber, $"Json Level Error ({nextLevelDiff})");

                if ((!dataType.StartsWith("object")) && (nextLevelDiff > 0))
                    throw new JsonTagParseException(lineNumber, $"Json Tag DataType Must Be [object]");

                
                if (!isArray)
                {
                    currJsonObject.Add(jsonTag.TagName, new Dictionary<string, object>());
                    jsonObjectTree.Add(currJsonObject[jsonTag.TagName] as Dictionary<string, object>);
                }
                else
                {
                    currJsonObject.Add(jsonTag.TagName, new List<Dictionary<string, object>>());
                    jsonObjectTree.Add(currJsonObject[jsonTag.TagName] as List<Dictionary<string, object>>);
                    (jsonObjectTree.Last() as List<Dictionary<string, object>>).Add(new Dictionary<string, object>());
                }
                    
            }
            return JsonConvert.SerializeObject(jsonObject);
        }
        /// <summary>
        /// Get Json Tag Description Value
        /// </summary>
        /// <param name="jsonTag"></param>
        /// <returns></returns>
        private string GetJsonTagDescription(JsonTag jsonTag)
        {
            var dataType = jsonTag.DataType.TrimEnd('[', ']');
            return $"({dataType}{(jsonTag.IsRequred ? "*" : "")}) {jsonTag.Description}";
        }
        /// <summary>
        /// Convert Line Text To <see cref="JsonTag"/>
        /// </summary>
        /// <param name="lineNumber"></param>
        /// <param name="lineText"></param>
        /// <returns></returns>
        private JsonTag GetJsonTagByLineText(int lineNumber, string lineText)
        {
            var cols = lineText.Trim().Split('\t');
            if (cols.Length != 4)
                throw new JsonTagParseException(lineNumber, $"Invalid Json Line Text: {lineText}");
            var jsonTag = new JsonTag();
            jsonTag.Level = 0;
            jsonTag.TagName = cols[0].Replace(" ", "").Trim();
            jsonTag.DataType = cols[1];
            jsonTag.IsRequred = cols[2].Equals("Y");
            jsonTag.Description = cols[3];

            var colNamePattern = @"([\+]{0,5})([a-zA-Z].*)";
            var match = Regex.Match(jsonTag.TagName, colNamePattern);
            if (match == null)
                throw new JsonTagParseException(lineNumber, $"Invalid Json Tag: {jsonTag.TagName}");

            if (match.Groups.Count == 1)
                jsonTag.TagName = match.Value;
            else
            {
                jsonTag.Level = match.Groups[1].Value.Length;
                jsonTag.TagName = match.Groups[2].Value;
            }
            return jsonTag;
        }

        private void rdoHtml_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
    
}
