using System;
using System.Collections.Generic;

namespace RogalikGame
{
    class Program
    {
        static Random rnd = new Random();

        class Weapon
        {
            public string nazvanie;
            public int uron;
            public int prochnost;
            public Weapon(string n, int u, int p)
            {
                nazvanie = n;
                uron = u;
                prochnost = p;
            }
        }

        class Aid
        {
            public string nazvanie;
            public int lechenie;
            public Aid(string n, int l)
            {
                nazvanie = n;
                lechenie = l;
            }
        }

        class Player
        {
            public string imya;
            public int hpCurrent;
            public int hpMax;
            public Weapon oruzhie;
            public Aid aptechka;
            public int ochki;
            public int uroven;
            public List<Tuple<string, int, int, int>> AvailableAbilities = new List<Tuple<string, int, int, int>>();


            public Player(string n, Weapon w, Aid a)
            {
                imya = char.ToUpper(n[0]) + n.Substring(1);
                oruzhie = w;
                aptechka = a;
                hpMax = 100;
                hpCurrent = hpMax;
                ochki = 0;
                uroven = 1;
            }

            public void UrovenCheck()
            {
                int newUroven = ochki / 50 + 1;
                if (newUroven > uroven)
                {
                    Console.WriteLine("----------------------------------");
                    Console.WriteLine($"Вы повысили уровень! Уровень: {newUroven}");
                    uroven = newUroven;
                    hpMax += 10;
                    hpCurrent += 10;
                    Console.WriteLine($"Ваше здоровье увеличилось до {hpMax} HP");

                    foreach (var ab in AvailableAbilities)
                    {
                        if (ab.Item3 == uroven)
                        {
                            Console.WriteLine($"Теперь доступна новая спецспособность: {ab.Item1}");
                        }
                    }
                    Console.WriteLine("----------------------------------");
                }
            }

            public void Heal()
            {
                int minHeal = (int)Math.Round(aptechka.lechenie * 0.6);
                int maxHeal = (int)Math.Round(aptechka.lechenie * 1.4);
                double buff = rnd.NextDouble() * 0.8 + 0.6;
                int healValue = (int)Math.Round(aptechka.lechenie * buff);

                Console.WriteLine($"Аптечка восстанавливает от {minHeal} до {maxHeal} HP. Ваш бросок коэфицента: {buff:F2}");
                hpCurrent += healValue;
                if (hpCurrent > hpMax) hpCurrent = hpMax;
                Console.WriteLine($"{imya} использовал аптечку. Текущее здоровье: {hpCurrent} HP");
            }

            public void Attack(Enemy vrag)
            {
                int kost = rnd.Next(1, 21);
                double buff = rnd.NextDouble() * 0.8 + 0.6;
                int min = (int)Math.Round(oruzhie.uron * 0.6);
                int max = (int)Math.Round(oruzhie.uron * 1.4);

                Console.WriteLine($"Диапазон урона: {min} – {max}");
                Console.WriteLine($"Бросок кости: {kost}, коэффициент: {buff:F2}");

                int uronFinal = (int)Math.Round(oruzhie.uron * buff);
                vrag.hpCurrent -= uronFinal;
                if (vrag.hpCurrent < 0) vrag.hpCurrent = 0;

                Console.WriteLine($"{imya} наносит {uronFinal} урона {vrag.nazvanie}");
            }

            public void SpecMolniya(List<Enemy> vragi, int duration)
            {
                Console.WriteLine($"Используется: Разряд молнии (-20 HP всем врагам) на {duration} ходов");
                foreach (var v in vragi)
                {
                    int kost = rnd.Next(1, 21);
                    double buff = rnd.NextDouble() * 0.8 + 0.6;
                    int dmg = (int)Math.Round(20 * buff);

                    v.hpCurrent -= dmg;
                    if (v.hpCurrent < 0) v.hpCurrent = 0;

                    Console.WriteLine($"{v.nazvanie} получает {dmg} урона (кость {kost})");
                }
            }

            public void SpecYad(List<Enemy> vragi, int duration)
            {
                Console.WriteLine($"Используется: Ядовитое зелье (-10 HP всем врагам) на {duration} ходов");
                foreach (var v in vragi)
                {
                    v.hpCurrent -= 10;
                    if (v.hpCurrent < 0) v.hpCurrent = 0;
                    Console.WriteLine($"{v.nazvanie} получает 10 урона от яда");
                }
            }
            public void SpecOgon(List<Enemy> vragi, int duration)
            {
                Console.WriteLine($"Используется: Огненный шар (-70 HP всем врагам) на {duration} ходов");
                foreach (var v in vragi)
                {
                    int kost = rnd.Next(1, 21);
                    double buff = rnd.NextDouble() * 0.8 + 0.6;
                    int dmg = (int)Math.Round(70 * buff);

                    v.hpCurrent -= dmg;
                    if (v.hpCurrent < 0) v.hpCurrent = 0;

                    Console.WriteLine($"{v.nazvanie} получает {dmg} урона (кость {kost})");
                }
            }

            public void SpecIscelenie(List<Enemy> vragi, int duration)
            {
                Console.WriteLine($"Используется: Исцеление (+15 HP игроку за ход) на {duration} ходов");
                int kost = rnd.Next(1, 21);
                double buff = rnd.NextDouble() * 0.8 + 0.6;
                int healValue = (int)Math.Round(15 * buff);
                hpCurrent += healValue;
                if (hpCurrent > hpMax) hpCurrent = hpMax;
                Console.WriteLine($"{imya} восстанавливает {healValue} HP (кость {kost}, коэффициент {buff:F2})");

                foreach (var v in vragi)
                {
                    if (v.nazvanie == "Скелет" || v.nazvanie == "Демон")
                    {
                        v.hpCurrent -= 40;
                        if (v.hpCurrent < 0) v.hpCurrent = 0;
                        Console.WriteLine($"{v.nazvanie} получает 40 урона от света");
                    }
                }
            }
        } 

        class Enemy
        {
            public string nazvanie;
            public int hpCurrent;
            public int hpMax;
            public Weapon oruzhie;
            public int uroven;

            public Enemy(string n, int hp, Weapon w, int u)
            {
                nazvanie = n;
                hpMax = hp;
                hpCurrent = hp;
                oruzhie = w;
                uroven = u;
            }

            public void Hod(Player igrok, List<Enemy> vseVragi)
            {
                if (hpCurrent <= 0) return;

                int action = rnd.Next(1, 4); 
                int kost = rnd.Next(1, 21);
                double buff = rnd.NextDouble() * 0.8 + 0.6;
                int uron;

                switch (action)
                {
                    case 1:
                        uron = (int)Math.Round(oruzhie.uron * buff);
                        igrok.hpCurrent -= uron;
                        if (igrok.hpCurrent < 0) igrok.hpCurrent = 0;
                        Console.WriteLine($"{nazvanie} атакует {igrok.imya} ({uron} урона, бросок кости {kost}, коэффициент {buff:F2})");
                        break;

                    case 2:
                        uron = (int)Math.Round(10 * uroven * buff);
                        igrok.hpCurrent -= uron;
                        if (igrok.hpCurrent < 0) igrok.hpCurrent = 0;
                        Console.WriteLine($"{nazvanie} использует спецспособность ({uron} урона, бросок кости {kost}, коэффициент {buff:F2})");
                        break;

                    case 3:
                        int heal = 10;
                        hpCurrent += heal;
                        if (hpCurrent > hpMax) hpCurrent = hpMax;
                        Console.WriteLine($"{nazvanie} использует лечение и восстанавливает {heal} HP (текущее здоровье {hpCurrent})");
                        break;
                }
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Добро пожаловать, воин!");
            Console.WriteLine("Назови себя:");
            string inputImya = Console.ReadLine();

            Weapon igrokOruz = new Weapon("Меч Фламберг", 20, 100);
            Aid igrokAid = new Aid("Средняя аптечка", 30);
            Player igrok = new Player(inputImya, igrokOruz, igrokAid);

            igrok.AvailableAbilities.Add(Tuple.Create("Разряд молнии", 20, 2, 3));
            igrok.AvailableAbilities.Add(Tuple.Create("Ядовитое зелье", 10, 5, 5));
            igrok.AvailableAbilities.Add(Tuple.Create("Огненный шар", 70, 10, 9));
            igrok.AvailableAbilities.Add(Tuple.Create("Исцеление", 15, 15, 3));

            List<string> imenaVragov = new List<string> { "Варвар", "Скелет", "Демон", "Орк", "Гоблин", "Зомби" };
            List<Weapon> oruzhieVragov = new List<Weapon>
            {
                new Weapon("Экскалибур",10,100),
                new Weapon("Топор",15,80),
                new Weapon("Кинжал",8,60),
                new Weapon("Посох",12,90),
                new Weapon("Булавка",5,50),
                new Weapon("Молот",18,100),
                new Weapon("Копьё",14,70)
            };

            Console.WriteLine($"Ваше имя {igrok.imya}!");
            Console.WriteLine($"HP: {igrok.hpCurrent}/{igrok.hpMax}, Оружие: {igrok.oruzhie.nazvanie} ({igrok.oruzhie.uron}), Аптечка: {igrok.aptechka.nazvanie} ({igrok.aptechka.lechenie} HP)");
            Console.WriteLine("----------------------------------");
            while (igrok.hpCurrent > 0)
            {
                int kolVragov = rnd.Next(1, 4);
                List<Enemy> vragi = new List<Enemy>();

                for (int i = 0; i < kolVragov; i++)
                {
                    if (rnd.Next(1, 101) <= 15)
                    {
                        double koeff = rnd.NextDouble() * 1.5 + 0.5;
                        int uronNov = (int)Math.Round(igrok.hpCurrent * koeff);
                        Weapon loot = new Weapon($"Меч силы ({uronNov} урона)", uronNov, 100);
                        Console.WriteLine($"Вы нашли оружие: {loot.nazvanie} (урон {loot.uron}). Заменить текущее оружие? (1-Да, 2-Нет)");
                        string choiceLoot = Console.ReadLine();
                        if (choiceLoot == "1")
                        {
                            igrok.oruzhie = loot;
                            Console.WriteLine("Оружие заменено!");
                        }
                        else
                        {
                            Console.WriteLine("Вы решили оставить текущее оружие.");
                        }
                        continue;
                    }

                    string vragName = imenaVragov[rnd.Next(imenaVragov.Count)];
                    Weapon vragOruz = oruzhieVragov[rnd.Next(oruzhieVragov.Count)];
                    int vragUroven = Math.Max(1, igrok.uroven + rnd.Next(-2, 3));
                    int vragHp = (int)Math.Round((30 + vragUroven * 10) * Math.Pow(1.1, vragUroven - 1));
                    vragi.Add(new Enemy(vragName, vragHp, vragOruz, vragUroven));
                }

                if (vragi.Count == 0) continue;

                Console.WriteLine($"\n--------- Встреча ---------");
                Console.WriteLine($"{igrok.imya} встречает {vragi.Count} врагов!");

                while (igrok.hpCurrent > 0 && vragi.Exists(v => v.hpCurrent > 0))
                {
                    Console.WriteLine("----------------------------------");
                    foreach (var vrag in vragi)
                    {
                        if (vrag.hpCurrent <= 0) continue;
                        Console.WriteLine($"{vrag.nazvanie} (Уровень {vrag.uroven}, HP {vrag.hpCurrent}/{vrag.hpMax}), оружие: {vrag.oruzhie.nazvanie} ({vrag.oruzhie.uron})");
                    }
                    Console.WriteLine($"Ваши параметры: HP {igrok.hpCurrent}/{igrok.hpMax}, уровень {igrok.uroven}, очки {igrok.ochki}");
                    Console.WriteLine("Выберите действие:");

                    for (int i = 0; i < vragi.Count; i++)
                    {
                        Enemy vrag = vragi[i];
                        if (vrag.hpCurrent <= 0) continue;

                        int minUron = (int)Math.Round(igrok.oruzhie.uron * 0.6);
                        int maxUron = (int)Math.Round(igrok.oruzhie.uron * 1.4);
                        Console.WriteLine($"{i + 1}. Атаковать {vrag.nazvanie} ({minUron}~{maxUron})");
                    }
                    if (igrok.AvailableAbilities.Count > 0)
                        Console.WriteLine($"{vragi.Count + 1}. Спецспособность (открывает меню спецспособностей)");

                    int minHeal = (int)Math.Round(igrok.aptechka.lechenie * 0.6);
                    int maxHeal = (int)Math.Round(igrok.aptechka.lechenie * 1.4);
                    Console.WriteLine($"{vragi.Count + 2}. Использовать аптечку ({minHeal}~{maxHeal})");

                    string choice = Console.ReadLine();
                    int choiceNum;
                    if (!int.TryParse(choice, out choiceNum) || choiceNum < 1 || choiceNum > vragi.Count + 2)
                    {
                        Console.WriteLine("Неверный выбор!");
                        continue;
                    }

                    if (choiceNum <= vragi.Count)
                    {
                        igrok.Attack(vragi[choiceNum - 1]);
                    }
                    else if (choiceNum == vragi.Count + 1)
                    {
                        bool choosingAbility = true;
                        while (choosingAbility)
                        {
                            Console.WriteLine("Выберите спецспособность:");
                            for (int j = 0; j < igrok.AvailableAbilities.Count; j++)
                            {
                                var ab = igrok.AvailableAbilities[j];
                                Console.WriteLine($"{j + 1}. {ab.Item1} (урон/эффект {ab.Item2} HP, длительность {ab.Item4} ходов, требуется уровень {ab.Item3})");
                            }
                            Console.WriteLine($"{igrok.AvailableAbilities.Count + 1}. Вернуться в главное меню");

                            string spChoice = Console.ReadLine();
                            int spNum;
                            if (int.TryParse(spChoice, out spNum))
                            {
                                if (spNum >= 1 && spNum <= igrok.AvailableAbilities.Count)
                                {
                                    var ab = igrok.AvailableAbilities[spNum - 1];
                                    if (igrok.uroven >= ab.Item3)
                                    {
                                        switch (ab.Item1)
                                        {
                                            case "Разряд молнии": igrok.SpecMolniya(vragi, ab.Item4); break;
                                            case "Ядовитое зелье": igrok.SpecYad(vragi, ab.Item4); break;
                                            case "Огненный шар": igrok.SpecOgon(vragi, ab.Item4); break;
                                            case "Исцеление": igrok.SpecIscelenie(vragi, ab.Item4); break;
                                        }
                                        choosingAbility = false;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Способность недоступна по уровню!");
                                    }
                                }
                                else if (spNum == igrok.AvailableAbilities.Count + 1)
                                {
                                    choosingAbility = false;
                                }
                                else
                                {
                                    Console.WriteLine("Неверный выбор спецспособности!");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Введите номер спецспособности!");
                            }
                        }
                    }
                    else
                    {
                        igrok.Heal();
                    }


                    Console.WriteLine("\n--------- ХОД ВРАГОВ ---------");
                    foreach (var v in vragi)
                        if (v.hpCurrent > 0)
                            v.Hod(igrok, vragi);
                }

                if (igrok.hpCurrent <= 0)
                {
                    Console.WriteLine("\nВы погибли...");
                    Console.WriteLine($"Очков набрано: {igrok.ochki}");
                    break;
                }

                int nagrada = vragi.Count * 10;
                igrok.ochki += nagrada;
                igrok.UrovenCheck();
                Console.WriteLine("----------------------------------");
                Console.WriteLine($"Вы победили! +{nagrada} очков");
                Console.WriteLine($"Всего очков: {igrok.ochki}");
                Console.WriteLine("----------------------------------");
            }

            Console.WriteLine("Игра окончена");
        }
    }
}
