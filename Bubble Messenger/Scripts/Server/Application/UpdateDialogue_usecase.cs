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
            SAME_NAME,
            EMPTY_NAME,
            EMPTY_PARTICIPANT,
            DIALOGUE_NOT_FOUND,
            USER_NOT_FOUND,
            NO_PARTICIPANT,
            ALREADY_PARTICIPANT,
            ALREADY_MUTED,
            ALREADY_BANNED,
            ALREADY_ADMIN,
            NOT_PARTICIPANT,
            ERROR
        }

        public UpdateDialogueUsecase(IDialogueRepo repo, IUserRepo user_repo)
        {
            Repo = repo;
            UserRepo = user_repo;
        }

        public Result Rename(string id, string new_name)
        {
            if (Repo.GetDialogueById(id) == null)
            {
                Console.WriteLine($"\nDialogue id={id} not found. (Rename attempt)");

                return Result.DIALOGUE_NOT_FOUND;
            }
            else if (Repo.GetDialogueById(id).Name == new_name)
            {
                Console.WriteLine($"\nDialogue id={id} already has the name \"{new_name}\". (Rename attempt)");

                return Result.SAME_NAME;
            }
            else if (new_name == "")
            {
                Console.WriteLine($"\nDialogue id={id} cannot have the empty name. (Rename attempt)");

                return Result.EMPTY_NAME;
            }
            else
            {
                Dialogue _target = Repo.GetDialogueById(id);
                string _deadDialogueName = _target.Name;

                _target.Name = new_name;

                Repo.UpdateDialogue(_target);

                Console.WriteLine($"\nDialogue {_deadDialogueName} renamed successfully.\nNew name: {new_name}");

                return Result.SUCCESS;
            }
        }

        public Result ChangeParticipants(string dialogue_id, string type, string participant_name)
        {
            if (UserRepo.GetUserByName(participant_name) == null)
            {
                return Result.USER_NOT_FOUND;
            }

            if (type == "add")
            {
                if (!Repo.GetDialogueById(dialogue_id).Participants.Contains(UserRepo.GetUserByName(participant_name)) && participant_name != null)
                {
                    return Result.ALREADY_PARTICIPANT;
                }
                else
                {
                    Repo.GetDialogueById(dialogue_id).Participants.Add(UserRepo.GetUserByName(participant_name));

                    return Result.SUCCESS;
                }
            }

            else if (type == "remove")
            {
                if (Repo.GetDialogueById(dialogue_id).Participants.Contains(UserRepo.GetUserByName(participant_name)) && participant_name != null)
                {
                    Repo.GetDialogueById(dialogue_id).Participants.Remove(UserRepo.GetUserByName(participant_name));

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

        public Result ChangeAdministrators(string dialogue_id, string type, string participant_name)
        {
            if (UserRepo.GetUserByName(participant_name) == null)
            {
                return Result.USER_NOT_FOUND;
            }

            if (type == "add")
            {
                if (!Repo.GetDialogueById(dialogue_id).Administrators.Contains(UserRepo.GetUserByName(participant_name)) && participant_name != null)
                {
                    return Result.ALREADY_ADMIN;
                }
                else
                {
                    Repo.GetDialogueById(dialogue_id).Administrators.Add(UserRepo.GetUserByName(participant_name));

                    return Result.SUCCESS;
                }
            }

            else if (type == "remove")
            {
                if (Repo.GetDialogueById(dialogue_id).Administrators.Contains(UserRepo.GetUserByName(participant_name)) && participant_name != null)
                {
                    Repo.GetDialogueById(dialogue_id).Administrators.Remove(UserRepo.GetUserByName(participant_name));

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

        public Result ChangeMuted(string dialogue_id, string type, string participant_name)
        {
            if (UserRepo.GetUserByName(participant_name) == null)
            {
                return Result.USER_NOT_FOUND;
            }

            if (type == "add")
            {
                if (!Repo.GetDialogueById(dialogue_id).Muted.Contains(UserRepo.GetUserByName(participant_name)) && participant_name != null)
                {
                    return Result.ALREADY_MUTED;
                }
                else
                {
                    Repo.GetDialogueById(dialogue_id).Muted.Add(UserRepo.GetUserByName(participant_name));

                    return Result.SUCCESS;
                }
            }

            else if (type == "remove")
            {
                if (Repo.GetDialogueById(dialogue_id).Muted.Contains(UserRepo.GetUserByName(participant_name)) && participant_name != null)
                {
                    Repo.GetDialogueById(dialogue_id).Muted.Remove(UserRepo.GetUserByName(participant_name));

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

        public Result ChangeBanned(string dialogue_id, string type, string participant_name)
        {
            if (UserRepo.GetUserByName(participant_name) == null)
            {
                return Result.USER_NOT_FOUND;
            }

            if (type == "add")
            {
                if (!Repo.GetDialogueById(dialogue_id).Banned.Contains(UserRepo.GetUserByName(participant_name)) && participant_name != null)
                {
                    return Result.ALREADY_BANNED;
                }
                else
                {
                    Repo.GetDialogueById(dialogue_id).Banned.Add(UserRepo.GetUserByName(participant_name));

                    return Result.SUCCESS;
                }
            }

            else if (type == "remove")
            {
                if (Repo.GetDialogueById(dialogue_id).Banned.Contains(UserRepo.GetUserByName(participant_name)) && participant_name != null)
                {
                    Repo.GetDialogueById(dialogue_id).Banned.Remove(UserRepo.GetUserByName(participant_name));

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
