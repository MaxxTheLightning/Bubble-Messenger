namespace Domain
{
    public interface IDialogueRepo
    {
        public Dialogue GetDialogueById(string id);

        public void CreateDialogue(string name, string type, User creator);

        public void DeleteDialogue(string id);

        public void UpdateDialogue(Dialogue dialogue);
    }
}