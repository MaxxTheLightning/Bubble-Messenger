namespace Domain
{
    public interface IDialogueRepo
    {
        public Dialogue GetDialogue(string id);

        public void CreateDialogue(string name, string type);

        public void DeleteDialogue(string id);

        public void UpdateDialogue(Dialogue dialogue);
    }
}