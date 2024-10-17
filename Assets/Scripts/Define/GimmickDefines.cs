namespace Defines
{
    public static class GimmickDefines
    {
        /// <summary> 기믹 오브젝트와 충돌 시 상호작용하는 타겟. (Layer와 일치 하도록 설정) </summary>
        public enum TargetType
        {
            Player,
            SkillA,
            SkillB,
        }

        public enum PlacementModeType
        {
            Gimmick,
            CameraPath,
        }

        public enum CameraPathInsertMode
        {
            None,
            Add,
            Insert,
        }
    }
}