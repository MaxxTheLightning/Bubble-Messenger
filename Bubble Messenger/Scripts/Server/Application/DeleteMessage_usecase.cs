using Domain;

namespace Application
{
    public class DeleteMessageUsecase
    {
        IMessageRepo Repo { get; }

        public enum Result
        {
            SUCCESS,
            MESSAGE_DOESNT_EXIST
        }

        public DeleteMessageUsecase(IMessageRepo repo)
        {
            Repo = repo;
        }

        public Result Execute(string id)
        {
            Message _message = Repo.GetMessage(id);  // Пробуем получить сообщение

            if (_message != null)
            {
                Repo.DeleteMessage(id);
                return Result.SUCCESS;
            }
            else
            {
                return Result.MESSAGE_DOESNT_EXIST;
            }
        }
    }
}
