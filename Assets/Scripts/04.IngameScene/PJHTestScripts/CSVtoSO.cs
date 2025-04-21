using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


public static class CSVtoSO
{
    private static string path = "/Resources/CSVFile/ItemInfo.csv";

    [MenuItem("Tools/CSVtoSO")]
    public static void GenerateItems()
    {
        
        string[] allLines = File.ReadAllLines(Application.dataPath + path);
        Items items = ScriptableObject.CreateInstance<Items>();
        //if()
        foreach(string allLine in allLines)
        {
            string[] splitData = allLine.Split(',');
            
            
            
        }
        
    }
}

public static class SOtoCSV
{
    private static string outputPath = "/Resources/CSVFile/";

    [MenuItem("Tools/Export ScriptableObject to CSV")]
    public static void ExportSkillDataToCSV()
    {
        // CSV 파일 저장 경로
        
        string[] targets = { "Movement", "PistolSkills", "SwordSkills" };
        string fullPath = Application.dataPath + outputPath;
        // CSV 파일 작성 준비
        using (StreamWriter writer = new StreamWriter(fullPath))
        {
            // CSV 헤더 작성
            writer.WriteLine("SkillName,Description,SkillType,Rarity,Damage,Cooldown,Range,Duration,Price");
            
            // 스크립터블 오브젝트 검색
            string[] guids = AssetDatabase.FindAssets("t:SkillObject");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                int lastIndex = path.LastIndexOf('/');
                if(!targets.Contains(path.Split('/')[lastIndex - 1]))
                {
                    continue;
                }
                
                SkillObject skillObject = AssetDatabase.LoadAssetAtPath<SkillObject>(path);

                // 스킬 데이터 작성
                // foreach (var skill in skillObject.skills)
                // {
                //     string line = $"{skill.skillName},{skill.description},{skill.skillType}," +
                //                   $"{skill.damage},{skill.cooldown},{skill.range},{skill.duration},{skill.price}";
                //     writer.WriteLine(line);
                // }
            }
        }

        // 완료 메시지
        Debug.Log($"Skill data exported to: {fullPath}");
    }
}
