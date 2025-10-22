//Подсчёт потраченных за день средств
using System.ComponentModel.DataAnnotations;
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
        Console.WriteLine($"\n{productOrService[i]} - {cost[i]} руб.");
    }
}

void statistics(string[] productOrService, int[] cost, int quantity)
{

    int average = 0, max = 0, min = 100000, sum = 0;

    for (int i = 0; i < quantity; i++)
    {
        average += cost[i];
        if (min > cost[i]) min = cost[i];
        if (max < cost[i]) max = cost[i];
        sum += cost[i];
    }

    average /= quantity;

    Console.WriteLine($"\nСреднее: {average} \nСумма трат: {sum} \nМинимальная трата: {min} \nМаксимальная трата: {max}");
}

void BubbleSort(string[] productOrService, int[] cost, int quantity)
{
    for (int i = 0; i < quantity; i++)
    {
        for (int j = 0; j < quantity - 1; j++)
        {
            if (cost[i] < cost[j])
            {
                int temp = cost[i];
                cost[i] = cost[j];
                cost[j] = temp;
                string temp2 = productOrService[i];
                productOrService[i] = productOrService[j];
                productOrService[j] = temp2;
            }
        }
    }
}

void CurrencyConverter(int[] cost, int quantity)
{
    Console.WriteLine("\nВыберите в какую валюту хотите перевести: ");
    Console.WriteLine("1. Доллар");
    Console.WriteLine("2. Евро");
    Console.WriteLine("3. Тенге");
    Console.WriteLine("4. Своя валюта");

    decimal[] costcopy = new decimal[quantity];
    for (int i = 0; i < quantity; i++)
    {
        costcopy[i] = cost[i];
    }

    int n = Convert.ToInt32(Console.ReadLine());
    string choice = "";
    decimal usdRate = 81.35m;
    decimal eurRate = 94.67m;
    decimal KZTRate = 0.1512m;
    decimal perRate = 0;

    if (n == 1)
    {
        choice = "USD";
        for (int i = 0; i < quantity; i++)
        {

            costcopy[i] = (decimal)(costcopy[i] / usdRate);
        }
    }
    if (n == 2)
    {
        choice = "EUR";
        for (int i = 0; i < quantity; i++)
        {
            costcopy[i] = (decimal)(costcopy[i] / eurRate);
        }
    }
    if (n == 3)
    {
        choice = "KZT";
        for (int i = 0; i < quantity; i++)
        {
            costcopy[i] = (decimal)(costcopy[i] / KZTRate);
        }
    }
    if (n == 4)
    {
        Console.WriteLine("\nВведите курс по отношению к рублю: ");
        perRate = Convert.ToDecimal(Console.ReadLine());
        for (int i = 0; i < quantity; i++)
        {
            costcopy[i] = (decimal)(costcopy[i] / perRate);
        }
    }
    for (int i = 0; i < quantity; i++)
    {
        Console.WriteLine($"\n{productOrService[i]} - {costcopy[i]} {choice}");
    }
}

void searchByName(string[] producrOrService, int[] cost, int quantity)
{
    Console.WriteLine("\nВведите товар или услугу стоимость которой хотите увидеть: ");
    string pos = Console.ReadLine();
    for (int i = 0; i < quantity; i++)
    {
        if (pos == producrOrService[i])
        {
            Console.WriteLine($"\n{cost[i]}");
        }
    }
}

while (true)
{
    Console.WriteLine("\n========= Меню =========");
    Console.WriteLine("1. Вывод данных");
    Console.WriteLine("2. Статистика");
    Console.WriteLine("3. Сортировка по цене");
    Console.WriteLine("4. Конвертация валюты");
    Console.WriteLine("5. Поиск по названию");
    Console.WriteLine("0. Выход");

    Console.WriteLine("\nВыберите пункт: ");
    int n = Convert.ToInt32(Console.ReadLine());

    switch (n)
    {
        case 1:
            output(productOrService, cost, quantity);
            break;
        case 2:
            statistics(productOrService, cost, quantity);
            break;
        case 3:
            BubbleSort(productOrService, cost, quantity);
            break;
        case 4:
            CurrencyConverter(cost, quantity);
            break;
        case 5:
            searchByName(productOrService, cost, quantity);
            break;
        case 0:
            return;
        default:
            Console.WriteLine("\nТакого варианта нет!");
            break;
    }
}
    
