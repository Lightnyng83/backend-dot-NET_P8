using TripPricer.Helpers;

namespace TripPricer;

public class TripPricer
{
    //public List<Provider> GetPrice(string apiKey, Guid attractionId, int adults, int children, int nightsStay, int rewardsPoints)
    //{
    //    List<Provider> providers = new List<Provider>();
    //    HashSet<string> providersUsed = new HashSet<string>();

    //    // Sleep to simulate some latency
    //    //Thread.Sleep(ThreadLocalRandom.Current.Next(1, 50));

    //    for (int i = 0; i < 5; i++)
    //    {
    //        int multiple = ThreadLocalRandom.Current.Next(100, 700);
    //        double childrenDiscount = children / 3.0;
    //        double price = multiple * adults + multiple * childrenDiscount * nightsStay + 0.99 - rewardsPoints;

    //        if (price < 0.0)
    //        {
    //            price = 0.0;
    //        }

    //        string provider;
    //        do
    //        {
    //            provider = GetProviderName(apiKey, adults);
    //        } while (providersUsed.Contains(provider));

    //        providersUsed.Add(provider);
    //        providers.Add(new Provider(attractionId, provider, price));
    //    }
    //    return providers;
    //}
    /*
     Le problème rencontré est que le pool de noms disponibles dans GetProviderName était trop petit (par exemple, seulement 9 noms différents étaient disponibles),
    ce qui faisait que lorsque l'on voulait générer 10 fournisseurs uniques, la boucle se bloquait en cherchant un nom encore jamais utilisé et finissait par tourner indéfiniment.
    Pourquoi l'ai-je supprimé dans l'exemple ?

Pour simplifier la logique et éviter le risque de boucle infinie, deux approches sont possibles :

    Augmenter le pool de noms uniques :
    Si l'unicité des fournisseurs est vraiment requise, il faut étendre la gamme de noms possibles (par exemple, en générant un nombre aléatoire de 1 à 10 au lieu de 1 à 9, et en mettant 
    à jour le switch pour renvoyer 10 noms différents). Dans ce cas, le HashSet peut continuer à être utilisé pour garantir l'unicité.

    Autoriser des doublons :
    Si l'unicité stricte des noms n'est pas indispensable pour la logique métier, on peut décider de supprimer la contrainte en éliminant le HashSet et la boucle do/while correspondante. 
    Cela permet de générer directement 10 fournisseurs, même si certains noms se répètent.*/

    public List<Provider> GetPrice(string apiKey, Guid attractionId, int adults, int children, int nightsStay, int rewardsPoints)
    {
        List<Provider> providers = new List<Provider>();

        for (int i = 0; i < 10; i++)
        {
            int multiple = ThreadLocalRandom.Current.Next(100, 700);
            double childrenDiscount = children / 3.0;
            double price = multiple * adults + multiple * childrenDiscount * nightsStay + 0.99 - rewardsPoints;

            if (price < 0.0)
            {
                price = 0.0;
            }

            // Sans vérifier l'unicité des noms :
            string provider = GetProviderName(apiKey, adults);
            providers.Add(new Provider(attractionId, provider, price));
        }
        return providers;
    }
    public string GetProviderName(string apiKey, int adults)
    {
        int multiple = ThreadLocalRandom.Current.Next(1, 10);

        return multiple switch
        {
            1 => "Holiday Travels",
            2 => "Enterprize Ventures Limited",
            3 => "Sunny Days",
            4 => "FlyAway Trips",
            5 => "United Partners Vacations",
            6 => "Dream Trips",
            7 => "Live Free",
            8 => "Dancing Waves Cruselines and Partners",
            9 => "AdventureCo",
            _ => "Cure-Your-Blues",
        };
    }
}
