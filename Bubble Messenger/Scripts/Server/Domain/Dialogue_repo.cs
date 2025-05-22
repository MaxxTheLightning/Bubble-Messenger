namespace Domain
{
    public interface IDialogueRepo
    {
        public Dialogue GetDialogueByName(string id);

        public void CreateDialogue(string name, string type);

        public void DeleteDialogue(string id);

        public void UpdateDialogue(Dialogue dialogue);
    }
}