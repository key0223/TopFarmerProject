using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public interface IMailItem 
{
    string Content { get; }
    MailType MailType { get; set; }
}
