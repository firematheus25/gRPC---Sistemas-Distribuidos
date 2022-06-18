using System.Linq.Expressions;
using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcClient;
using Task = System.Threading.Tasks.Task;


// The port number must match the port of the gRPC server.
using var channel = GrpcChannel.ForAddress("http://localhost:50051");
var client = new ToDo.ToDoClient(channel);


await menu(client);





async Task menu(ToDo.ToDoClient client)
{
    Console.WriteLine("\n1 - Listar tarefas.\n2 - Adicionar tarefa.\n3 - Sair.");
    Console.Write("\nEscolha uma ação: ");
    var response = Convert.ToInt32(Console.ReadLine());
    if (response != 1 && response != 2 && response != 3)
    {
        Console.WriteLine("Informe um valor válido");
        await menu(client);
    }

    switch (response)
    {
        case 1:
            await ListTasks(client);
            break;
        case 2:
            await AddTask(client);
            break;
        case 3:
            Environment.Exit(0);
            break;
    }

}



async Task AddTask(ToDo.ToDoClient client)
{
    try
    {
        Console.Clear();

        Console.Write("Informe a tarefa: ");
        var task = Console.ReadLine();
        Console.WriteLine("\nProcessando Requisição ...");

        await client.AddAsync(new GrpcClient.Task { Id = 1, Description = task });

        Console.WriteLine("\n\nTarefa Registrada com sucesso");

        await menu(client);
    }
    catch (Exception e)
    {
        Console.WriteLine("\nErro interno no servidor");
        await menu(client);
    }
}

async Task ListTasks(ToDo.ToDoClient client)
{
    try
    {
        Console.Clear();

        Console.Write("Processando Requisição ...");

        var reply = await client.ListAllAsync(new EmptyMessage { });

        Console.Clear();

        if (reply.Tasks.Count == 0)
        {
            Console.WriteLine("Nenhuma tarefa registrada.");
        }
        else
        {
            foreach (var tasks in reply.Tasks)
            {
                Console.Write($"Tarefa:{tasks.Id} \nDescrição: {tasks.Description} \n\n");
            }
        }


        await menu(client);
    }
    catch (Exception)
    {
        Console.WriteLine("\nErro interno no servidor");
        await menu(client);
    }

}
