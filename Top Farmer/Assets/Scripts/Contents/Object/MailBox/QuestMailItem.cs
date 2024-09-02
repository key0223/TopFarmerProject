using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class QuestMailItem : IMailItem
{
    public QuestData QuestData { get; private set; }
    public string Content => QuestData.questDescription;

    public MailType MailType { get; set; }

    public QuestMailItem(QuestData questData)
    {
        QuestData = questData;
        MailType = MailType.Quest;
    }
}
