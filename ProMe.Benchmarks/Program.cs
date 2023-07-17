using System.Text;

while (Console.ReadKey().KeyChar != 'e')

{
    var key = Guid.NewGuid();

    var string64 = Convert.ToBase64String(key.ToByteArray());

    Console.WriteLine(string64.Length);

}