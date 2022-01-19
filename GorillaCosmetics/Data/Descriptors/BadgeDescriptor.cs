using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace GorillaCosmetics.Data.Descriptors
{
    public class BadgeDescriptor : MonoBehaviour
    {
        public string BadgeName = "Badge";
        public string AuthorName = "Author";
        public string Description = string.Empty;
        public bool CustomColors = false;
        public bool DisablePublicLobbies = false;
        public AttachPoint attachPoint = AttachPoint.Chest;
    }
    public enum AttachPoint
    {
        RightHand,
        LeftHand,
        Chest
    }
}
