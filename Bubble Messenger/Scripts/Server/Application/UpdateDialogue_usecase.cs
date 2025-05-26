using Domain;

namespace Application
{
    public class UpdateDialogueUsecase
    {
        IDialogueRepo Repo { get; }

        IUserRepo UserRepo { get; }

        public enum Result
        {
            SUCCESS,
            SAME,
            EMPTY_NAME,
            EMPTY_PARTICIPANT,
            NOT_FOUND,
            USER_NOT_FOUND,
            NO_PARTICIPANT,
            ALREADY_PARTICIPANT,
            NOT_PARTICIPANT,
            ERROR
        }

        public UpdateDialogueUsecase(IDialogueRepo repo, IUserRepo user_repo)
        {
            Repo = repo;
            UserRepo = user_repo;
        }

        public Result ChangeName(string name, string new_name)
        {
            if (Repo.GetDialogueByName(name) == null)
            {
                return Result.NOT_FOUND;
            }
            else if (Repo.GetDialogueByName(name).Name == new_name)
            {
                return Result.SAME;
            }
            else if (new_name == "")
            {
                return Result.EMPTY_NAME;
            }
            else
            {
                Dialogue _target = Repo.GetDialogueByName(name);
                _target.Name = new_name;

                Repo.UpdateDialogue(_target);

                return Result.SUCCESS;
            }
        }

        public Result ChangeParticipants(string dialogue_name, string type, string participant)
        {
            if (dialogue_name == "")
            {
                return Result.EMPTY_NAME;
            }
            else if (participant == "")
            {
                return Result.EMPTY_PARTICIPANT;
            }
            else if (UserRepo.GetUserByName(participant) == null)
            {
                return Result.USER_NOT_FOUND;
            }

            if (type == "add")
            {
                if (!Repo.GetDialogueByName(dialogue_name).Participants.Contains(UserRepo.GetUserByName(participant)) && participant != null)
                {
                    return Result.ALREADY_PARTICIPANT;
                }
                else
                {
                    Repo.GetDialogueByName(dialogue_name).Participants.Add(UserRepo.GetUserByName(participant));

                    return Result.SUCCESS;
                }
            }
            else if (type == "remove")
            {
                if (Repo.GetDialogueByName(dialogue_name).Participants.Contains(UserRepo.GetUserByName(participant)) && participant != null)
                {
                    Repo.GetDialogueByName(dialogue_name).Participants.Remove(UserRepo.GetUserByName(participant));

                    return Result.SUCCESS;
                }
                else
                {
                    return Result.NOT_PARTICIPANT;
                }
            }
            else
            {
                return Result.ERROR;
            }
        }
    }
}
