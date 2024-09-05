using System;
using appLivros;
using MySql.Data.MySqlClient;

namespace GerenciamentoLivros
{
    class Program
    {
        private const string ConnectionString = "Server=localhost;Database=db_aulas_2024;Uid=shilton;Pwd=1234567;SslMode=none;";

        static void Main(string[] args)
        {
            while (true)
            {
                MostrarMenu();

                string opcao = Console.ReadLine();

                if (!ProcessarOpcaoMenu(opcao))
                {
                    break;
                }
            }
        }

        private static void MostrarMenu()
        {
            Console.WriteLine("1. Adicionar Livro");
            Console.WriteLine("2. Listar Livros");
            Console.WriteLine("3. Editar Livro");
            Console.WriteLine("4. Excluir Livro");
            Console.WriteLine("5. Buscar Livro por Autor ou Gênero");
            Console.WriteLine("6. Sair");
            Console.Write("Escolha uma opção: ");
        }

        private static bool ProcessarOpcaoMenu(string opcao)
        {
            switch (opcao)
            {
                case "1":
                    AdicionarLivro();
                    break;
                case "2":
                    ListarLivros();
                    break;
                case "3":
                    EditarLivro();
                    break;
                case "4":
                    ExcluirLivro();
                    break;
                case "5":
                    BuscarLivro();
                    break;
                case "6":
                    return false;
                default:
                    Console.WriteLine("Opção inválida!");
                    break;
            }
            return true;
        }

        private static void AdicionarLivro()
        {
            try
            {
                var livro = ObterDadosLivro();

                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO livros (Titulo, Autor, AnoPublicacao, Genero, NumeroPaginas) VALUES (@Titulo, @Autor, @AnoPublicacao, @Genero, @NumeroPaginas)";
                    var cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Titulo", livro.Titulo);
                    cmd.Parameters.AddWithValue("@Autor", livro.Autor);
                    cmd.Parameters.AddWithValue("@AnoPublicacao", livro.AnoPublicacao);
                    cmd.Parameters.AddWithValue("@Genero", livro.Genero);
                    cmd.Parameters.AddWithValue("@NumeroPaginas", livro.NumeroPaginas);
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine("Livro adicionado com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao adicionar livro: {ex.Message}");
            }
        }

        private static void ListarLivros()
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT Id, Titulo, Autor, AnoPublicacao, Genero, NumeroPaginas FROM livros";
                    var cmd = new MySqlCommand(query, connection);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("Nenhum livro cadastrado.");
                            return;
                        }

                        while (reader.Read())
                        {
                            Console.WriteLine($"ID: {reader["Id"]}, Título: {reader["Titulo"]}, Autor: {reader["Autor"]}, Ano: {reader["AnoPublicacao"]}, Gênero: {reader["Genero"]}, Páginas: {reader["NumeroPaginas"]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar livros: {ex.Message}");
            }
        }

        private static void EditarLivro()
        {
            try
            {
                int id = ObterIdLivro();

                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    var livroExistente = ObterLivroPorId(connection, id);
                    if (livroExistente == null)
                    {
                        Console.WriteLine("Livro não encontrado.");
                        return;
                    }

                    var livroAtualizado = AtualizarDadosLivro(livroExistente);

                    string query = "UPDATE livros SET Titulo = @Titulo, Autor = @Autor, AnoPublicacao = @AnoPublicacao, Genero = @Genero, NumeroPaginas = @NumeroPaginas WHERE Id = @Id";
                    var cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Titulo", livroAtualizado.Titulo);
                    cmd.Parameters.AddWithValue("@Autor", livroAtualizado.Autor);
                    cmd.Parameters.AddWithValue("@AnoPublicacao", livroAtualizado.AnoPublicacao);
                    cmd.Parameters.AddWithValue("@Genero", livroAtualizado.Genero);
                    cmd.Parameters.AddWithValue("@NumeroPaginas", livroAtualizado.NumeroPaginas);
                    cmd.Parameters.AddWithValue("@Id", livroAtualizado.Id);
                    cmd.ExecuteNonQuery();

                    Console.WriteLine("Livro editado com sucesso!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao editar livro: {ex.Message}");
            }
        }

        private static void ExcluirLivro()
        {
            try
            {
                int id = ObterIdLivro();

                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM livros WHERE Id = @Id";
                    var cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Id", id);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Livro excluído com sucesso!");
                    }
                    else
                    {
                        Console.WriteLine("Livro não encontrado.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao excluir livro: {ex.Message}");
            }
        }

        private static void BuscarLivro()
        {
            try
            {
                Console.Write("Digite o autor ou gênero para buscar: ");
                string termoBusca = Console.ReadLine();

                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT Id, Titulo, Autor, AnoPublicacao, Genero, NumeroPaginas FROM livros WHERE Autor LIKE @Termo OR Genero LIKE @Termo";
                    var cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Termo", "%" + termoBusca + "%");

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("Nenhum livro encontrado.");
                            return;
                        }

                        while (reader.Read())
                        {
                            Console.WriteLine($"ID: {reader["Id"]}, Título: {reader["Titulo"]}, Autor: {reader["Autor"]}, Ano: {reader["AnoPublicacao"]}, Gênero: {reader["Genero"]}, Páginas: {reader["NumeroPaginas"]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar livro: {ex.Message}");
            }
        }

        private static Livro ObterDadosLivro()
        {
            Console.Write("Título: ");
            string titulo = Console.ReadLine();
            Console.Write("Autor: ");
            string autor = Console.ReadLine();
            Console.Write("Ano de Publicação: ");
            int anoPublicacao = int.Parse(Console.ReadLine());
            Console.Write("Gênero: ");
            string genero = Console.ReadLine();
            Console.Write("Número de Páginas: ");
            int numeroPaginas = int.Parse(Console.ReadLine());

            return new Livro { Titulo = titulo, Autor = autor, AnoPublicacao = anoPublicacao, Genero = genero, NumeroPaginas = numeroPaginas };
        }

        private static Livro AtualizarDadosLivro(Livro livro)
        {
            Console.Write("Novo título (deixe em branco para não alterar): ");
            string titulo = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(titulo)) livro.Titulo = titulo;

            Console.Write("Novo autor (deixe em branco para não alterar): ");
            string autor = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(autor)) livro.Autor = autor;

            Console.Write("Novo ano de publicação (deixe em branco para não alterar): ");
            string anoInput = Console.ReadLine();
            if (int.TryParse(anoInput, out int anoPublicacao)) livro.AnoPublicacao = anoPublicacao;

            Console.Write("Novo gênero (deixe em branco para não alterar): ");
            string genero = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(genero)) livro.Genero = genero;

            Console.Write("Novo número de páginas (deixe em branco para não alterar): ");
            string paginasInput = Console.ReadLine();
            if (int.TryParse(paginasInput, out int numeroPaginas)) livro.NumeroPaginas = numeroPaginas;

            return livro;
        }

        private static int ObterIdLivro()
        {
            Console.Write("ID do livro: ");
            return int.Parse(Console.ReadLine());
        }

        private static Livro ObterLivroPorId(MySqlConnection connection, int id)
        {
            string query = "SELECT Id, Titulo, Autor, AnoPublicacao, Genero, NumeroPaginas FROM livros WHERE Id = @Id";
            var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new Livro
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Titulo = reader["Titulo"].ToString(),
                        Autor = reader["Autor"].ToString(),
                        AnoPublicacao = Convert.ToInt32(reader["AnoPublicacao"]),
                        Genero = reader["Genero"].ToString(),
                        NumeroPaginas = Convert.ToInt32(reader["NumeroPaginas"])
                    };
                }
            }
            return null;
        }
    }

   
}
