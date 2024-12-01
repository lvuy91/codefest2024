namespace CodeFest24.Model
{

    public class TreeNode
    {
        public int Val { get; set; }
        public string Dir { get; set; }
        public TreeNode Parent { get; set; }
        public List<TreeNode> Children { get; set; }
        public int Boxes { get; set; }
        public int IsolatedBoxes { get; set; }
        public bool AvoidThis { get; set; }
        public bool AttackThis { get; set; }
        public bool PlayerFootprint { get; set; }
        public int Distance { get; set; }
        public int BonusPoints { get; set; }

        public TreeNode(int val, string dir = null, TreeNode parent = null)
        {
            Val = val;
            Dir = dir;
            Parent = parent;
            Children = new List<TreeNode>();
            Boxes = 0;
            IsolatedBoxes = 0;
            AvoidThis = false;
            AttackThis = false;
            PlayerFootprint = false;

            if (parent != null)
            {
                Distance = parent.Distance + 1;
                BonusPoints = parent.BonusPoints;
            }
            else
            {
                Distance = 0;
                BonusPoints = 0;
            }
        }
    }
}
