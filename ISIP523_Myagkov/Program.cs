//Подсчёт потраченных за день средств
using System.Net.Http.Headers;

Console.WriteLine("Введите кол-во операций: ");
int quantity = Convert.ToInt32(Console.ReadLine());

string[] productOrService = new string[quantity];
int[] cost = new int[quantity];
for (int i = 0; i < quantity; i++)
{
    Console.WriteLine("Введите услугу или товар и стоимость по примеру(Влажные салфетки \"Лента\"; 235): ");
    string str = Console.ReadLine();
    string[] words = str.Split(new char[] { ';' });
    productOrService[i] = words[0];
    cost[i] = Convert.ToInt32(words[1]);

}

void output(string[] productOrService, int[] cost, int quantity)
{
    for (int i = 0;i < quantity; i++)
    {
        Console.WriteLine($"{productOrService[i]} - {cost[i]} руб.");
    }
}

Console.WriteLine("========= Меню =========");
Console.WriteLine("1. Вывод данных");
Console.WriteLine("2. Статистика");
Console.WriteLine("3. Сортировка по цене");
Console.WriteLine("4. Конвертация валюты");
Console.WriteLine("5. Поиск по названию");

for (int i = 0; i < 100000; i++)
{
    int n = Convert.ToInt32(Console.ReadLine());
    if (n == 1)
    {
        output(productOrService, cost, quantity);
    }

    if (n == 0)
    {
        break;
    }

