using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Extensions.UI.Builder;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Rules
{
    /// <summary>
    /// Summary description for GoldRulesTest
    /// </summary>
    [TestFixture]
    public class GoldRulesTest
    {
        [Test]
        [Ignore]
        //TODO-AC enbale when after Gold_Rules.sql and the PatientAccess.Rules.dll assembly is deployed with the test
        public void TestThatTheGoldRulesFileDoesNotHaveAnyRuleTypesThatAreNotPresentInTheCode()
        {
            String file = File.ReadAllText(@"C:\Source\PatientAccess\Trunk\04 Implementation\Source\PatientAccess\PatientAccess.Database\Oracle\Schema\GoldenData\Gold_Rules.sql");
            List<string> ruleTypesInSqlFile = this.GetRulesNameList(file);

            FileInfo fileInfo = new FileInfo("PatientAccess.Rules.dll");
            Assembly assembly = Assembly.LoadFile(fileInfo.FullName);

            var ruleTypesInAssembly = from type in assembly.GetTypes()
                                      where type.IsClass && type.IsSubclassOf(typeof(LeafRule))
                                      select type.FullName;

            var typesThatAreInTheAssemblyButNotInTheSqlFile = (from s in ruleTypesInAssembly
                                                               where !ruleTypesInSqlFile.Contains(s)
                                                               select s).ToList();

            var typesThatAreInTheSqlFileButNotInTheAssembly = (from s in ruleTypesInSqlFile
                                                               where !ruleTypesInAssembly.Contains(s)
                                                               select s).ToList();

            Trace.WriteLine("Types that are in the PatientAccess.Rules.dll assembly but not in the PatientAccess.Rules.dll assembly" + Environment.NewLine);
            
            foreach (var s in typesThatAreInTheAssemblyButNotInTheSqlFile)
            {
                Trace.WriteLine(s);
            }

            Trace.WriteLine("Types that are in the Gold_Rules.sql file but not in the PatientAccess.Rules.dll assembly"+Environment.NewLine);
            
            foreach (var s in typesThatAreInTheSqlFileButNotInTheAssembly)
            {
                Trace.WriteLine(s);
            }

            Assert.IsTrue(typesThatAreInTheSqlFileButNotInTheAssembly.Count==0);
        }

        private List<string> GetRulesNameList(string file)
        {
            var resultList = new List<String>();
            try
            {
                Regex regexObj = new Regex("PatientAccess.Rules.*',", RegexOptions.IgnoreCase);
                Match matchResult = regexObj.Match(file);
                while (matchResult.Success)
                {
                    string value = matchResult.Value;
                    value = value.Replace(",", string.Empty);
                    value = value.Replace("'", string.Empty);

                    resultList.Add(value);
                    matchResult = matchResult.NextMatch();

                }
            }
            catch
            {
                // Syntax error in the regular expression
            }
            return resultList;
        }
    }
}