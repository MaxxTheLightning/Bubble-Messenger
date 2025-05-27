using Domain;

namespace Infrastructure
{
    public class MockDialogueRepo : IDialogueRepo
    {
        public List<Dialogue> Dialogues { get; set; }

        IIdProvider IdProvider { get; }

        public MockDialogueRepo(IIdProvider idProvider) 
        {
            Dialogues = new List<Dialogue>();
            IdProvider = idProvider;
        }

        public Dialogue GetDialogueById(string id)
        {
            foreach (Dialogue dialogue in Dialogues.ToList())
            {
                if (dialogue.Id == id)
                {
                    return dialogue;
                }
            }

            return null;
        }

        public void CreateDialogue(string name, string type, User creator)
        {
            Dialogue new_dialogue = new Dialogue(IdProvider, name, type, creator);
            Dialogues.Add(new_dialogue);
        }

        public void DeleteDialogue(string id)
        {
            foreach (Dialogue dialogue in Dialogues.ToList())
            {
                if (dialogue.Id == id)
                {
                    Dialogues.Remove(dialogue);
                }
            }
        }

        public void UpdateDialogue(Dialogue dialogue)
        {
            foreach (Dialogue d in Dialogues.ToList())
            {
                if (d.Id == dialogue.Id)
                {
                    Dialogues[Dialogues.IndexOf(d)] = dialogue;
                }
            }
        }
    }
}
