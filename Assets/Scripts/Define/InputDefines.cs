using System;

namespace Defines
{
    public static class InputDefines
    {
        public enum ActionMapType
        {
            PlayerActions,
            UIActions,
        }

        public enum ActionPoint
        {
            IsStarted,
            IsPerformed,
            IsCanceled,
            All,
        }

        public enum SkillType
        {
            StarHunt,
            StarFusion,
            MAX
        }

        public struct InputActionName
        {
            public InputActionName(ActionMapType _mapType, string _actionName)
            {
                MapType = _mapType;
                ActionName = _actionName;
            }

            public ActionMapType MapType { get; }
            public string ActionName { get; }
        }

        public readonly static string Move = "Move";
        public readonly static string Jump = "Jump";
        public readonly static string Dash = "Dash";
        public readonly static string Magic = "Magic";

        public readonly static string CameraRotation = "CameraRotation";
    }
}