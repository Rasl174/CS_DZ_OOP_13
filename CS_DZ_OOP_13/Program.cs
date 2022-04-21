using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_DZ_OOP_13
{
    class Program
    {
        static void Main(string[] args)
        {
            List<ClientCar> clientsCars = new List<ClientCar>() { new ClientCar("Двигатель", 2000), new ClientCar("Колесо", 400), new ClientCar("Двигатель", 300), new ClientCar("Трансмиссия", 3000) };
            CarService carService = new CarService(1000);
            carService.Work(clientsCars);
        }
    }

    class CarService
    {
        private Storage _storage = new Storage();
        private List<Part> _partInService = new List<Part>();

        public int ServiceMoney { get; private set; }

        public CarService(int serviceMoney)
        {
            ServiceMoney = serviceMoney;
        }

        public void Work(List<ClientCar> clientsCars)
        {
            int fine = 500;

            while(clientsCars.Count > 0 && ServiceMoney > 0)
            {
                Console.WriteLine("В сервисе " + ServiceMoney + " денег");
                _storage.ShowParts();
                foreach (var client in clientsCars)
                {
                    Console.SetCursorPosition(0, 12);
                    Console.WriteLine("У меня сломалась деталь - " + client.RepairPartName + " и у меня есть " + client.Money + " денег");
                    Console.WriteLine("Возьметесь починить?");
                    Console.WriteLine("Введите 1 если да и 2 если нет");
                    string userinput = Console.ReadLine();

                    switch (userinput)
                    {
                        case "1":
                            Repair(client);
                            break;
                        case "2":
                            RepairFailure(client, fine);
                            break;
                    }
                    clientsCars.Remove(client);
                    break;
                }
                Console.ReadKey();
                Console.Clear();
            }
            Console.WriteLine("После последнего клиента у нас " + ServiceMoney + " денег");
        }

        private void RepairFailure(ClientCar client, int fine)
        {
            Console.WriteLine("Мы приносим извинения но мы не можем починить вашу деталь!");
            ServiceMoney -= fine;
            Console.WriteLine("Клиент со сломаной деталью - " + client.RepairPartName + " уехал!");
            Console.WriteLine("Он больше сюда не вернется!");
        }

        private void Repair(ClientCar client)
        {
            Console.WriteLine("Введите название детали на складе которую нужно взять для замены");
            _storage.TakePart(_partInService);
            foreach (var part in _partInService)
            {
                if(part.Name == client.RepairPartName && part.Price + part.ReplacementPrice <= client.Money)
                {
                    client.Pay(part.Price, part.ReplacementPrice);
                    client.PartReplacement(part.Name);
                    ServiceMoney += part.Price + part.ReplacementPrice;
                    _partInService.Remove(part);
                    Console.WriteLine("Клиент уехал с новой запчастью - " + client.RepairPartName + " и у него осталось " + client.Money + " денег");
                }
                else if(part.Name == client.RepairPartName && part.Price + part.ReplacementPrice > client.Money)
                {
                    Console.WriteLine("Слыш клиент я вижу что у тебя денег не хватает. Пока не оплатишь никуда не уедешь!");
                    client.BorrowMoney(part.Price, part.ReplacementPrice);
                    Console.WriteLine("Клиент занял денег на деталь и ремонт!");
                    client.Pay(part.Price, part.ReplacementPrice);
                    client.PartReplacement(part.Name);
                    ServiceMoney += part.Price + part.ReplacementPrice;
                    _partInService.Remove(part);
                    Console.WriteLine("Ну вот другой разговор машина уже готова!");
                    Console.WriteLine("Клиент уехал с новой запчастью - " + client.RepairPartName + " и у него осталось " + client.Money + " денег");

                }
                else if(part.Name != client.RepairPartName && part.Price + part.ReplacementPrice <= client.Money)
                {
                    Console.WriteLine("Вы издеваетесь! Мне нужна не эта деталь! Я отказываюсь платить за ремонт!");
                    client.PartReplacement(part.Name);
                    ServiceMoney -= part.Price + part.ReplacementPrice;
                    Console.WriteLine("Клиент остался не доволен он уехал со старой деталью " + client.RepairPartName + " зато с новой деталью " + part.Name);
                    _partInService.Remove(part);
                }
                break;
            }
        }
    }

    class Storage
    {
        private List<Part> _allParts = new List<Part>() { new Part("Двигатель", 1000, 500), new Part("Двигатель", 1000, 500), new Part("Колесо", 100, 100),
                                       new Part("Лампочка", 2, 2), new Part("Лампочка", 2, 2), new Part("Лампочка", 2, 2), new Part("Лампочка", 2, 2) };

        public void ShowParts()
        {
            if(_allParts.Count > 0)
            {
                Console.WriteLine("На складе есть");
                foreach (var part in _allParts)
                {
                    Console.WriteLine(part.Name + " с ценой - " + part.Price + " и с ценой работы - " + part.ReplacementPrice);
                }
            }
            else
            {
                Console.WriteLine("Склад пуст!");
            }
        }

        public void TakePart(List<Part> partInService)
        {
            bool correctInput = false;
            while(correctInput == false)
            {
                string userInput = Console.ReadLine();
                foreach (var part in _allParts)
                {
                    if(userInput == part.Name)
                    {
                        correctInput = true;
                        partInService.Add(part);
                        _allParts.Remove(part);
                        break;
                    }
                }
                if(partInService.Count == 0 && _allParts.Count > 0)
                {
                    Console.WriteLine("Нет такой детали повторите ввод!");
                }
            }
        }
    }

    class Part
    {
        public string Name { get; private set; }

        public int Price { get; private set; }

        public int ReplacementPrice { get; private set; }

        public Part(string name, int price, int replacementPrice)
        {
            Name = name;
            Price = price;
            ReplacementPrice = replacementPrice;
        }
    }

    class ClientCar
    {
        public string RepairPartName { get; private set; }

        public int Money { get; private set; }

        public ClientCar(string repairPartName, int money)
        {
            RepairPartName = repairPartName;
            Money = money;
        }

        public void Pay(int partPrice, int replacementPrice)
        {
            Money -= partPrice + replacementPrice;
        }

        public void PartReplacement(string newPart)
        {
            RepairPartName = newPart;
        }

        public void BorrowMoney(int partPrice, int replacementPrice)
        {
            Money += partPrice + replacementPrice;
        }
    }
}
