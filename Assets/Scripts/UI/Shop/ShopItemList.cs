using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
//using UnityEngine.Analytics.Experimental;
#endif

public class ShopItemList : ShopList
{
	static public Consumable.ConsumableType[] s_ConsumablesTypes = System.Enum.GetValues(typeof(Consumable.ConsumableType)) as Consumable.ConsumableType[];
    protected int additional,actualprice;
	public override void Populate()
    {
		m_RefreshCallback = null;
        foreach (Transform t in listRoot)
        {
            Destroy(t.gameObject);
        }

        for(int i = 0; i < s_ConsumablesTypes.Length; ++i)
        { int j = 0;
            additional = 0;
            Consumable c = ConsumableDatabase.GetConsumbale(s_ConsumablesTypes[i]);
            if(c != null)
            {
                GameObject newEntry = Instantiate(prefabItem);
                newEntry.transform.SetParent(listRoot, false);

                ShopItemListItem itm = newEntry.GetComponent<ShopItemListItem>();

				itm.buyButton.image.sprite = itm.buyButtonSprite;
                
				itm.nameText.text = c.GetConsumableName();
                if (c.upgradable)
                {
                    for (float o = 0f; o <= PlayerData.instance.pudm[c.identifier]; o += 2f)
                    {
                        int multi = (int)o;
                        if (multi > 0) { multi = multi / 2; }
                        additional += c.GetPrice() * multi;
                        itm.upbars[j].gameObject.SetActive(true);
                        j += 1;
                    }
                }
                 actualprice = c.GetPrice() + additional;
				itm.pricetext.text = actualprice.ToString();

				if (c.GetPremiumCost() > 0)
				{
					itm.premiumText.transform.parent.gameObject.SetActive(true);
					itm.premiumText.text = c.GetPremiumCost().ToString();
				}
				else
				{
					itm.premiumText.transform.parent.gameObject.SetActive(false);
				}

                if (c.GetPrice() > 0)
                {
                    itm.pricetext.transform.parent.gameObject.SetActive(true);
                    itm.pricetext.text = c.GetPrice().ToString();
                }
                else
                {
                    itm.pricetext.transform.parent.gameObject.SetActive(false);
                }

                itm.icon.sprite = c.icon;

				itm.countText.gameObject.SetActive(true);
                
                    itm.upButton.interactable=c.upgradable;
                itm.buyButton.onClick.AddListener(delegate () { Buy(c); });
                itm.upButton.onClick.AddListener(delegate () { Upgrade(c); });
                m_RefreshCallback += delegate () { RefreshButton(itm, c); };
				RefreshButton(itm, c);
			}
        }
    }

	protected void RefreshButton(ShopItemListItem itemList, Consumable c)
	{
        int j = 0;
        additional = 0;
		int count = 0;
		PlayerData.instance.consumables.TryGetValue(c.GetConsumableType(), out count);
		itemList.countText.text = count.ToString();
        if (c.upgradable)
        {
            for (float o = 0f; o <= PlayerData.instance.pudm[c.identifier]; o += 2f)
            {
                int multi = (int)o;
                if (multi > 0) { multi = multi / 2; }
                additional += c.GetPrice() * multi;
                if(j>0)
                itemList.upbars[j-1].gameObject.SetActive(true);
                j += 1;
            }
        }
        actualprice = c.GetPrice() + additional;
        itemList.pricetext.text = actualprice.ToString();

        if (PlayerData.instance.pudm[c.identifier] >= 8f)
        {
            itemList.upButton.interactable = false;
            itemList.desc.text = "MAX";

        }
		if (actualprice > PlayerData.instance.coins )
		{
			itemList.buyButton.interactable = false;
            itemList.upButton.interactable = false;
            itemList.pricetext.color = Color.red;
            
        }
		 else
		{
			itemList.pricetext.color = Color.black;
		}

		if (c.GetPremiumCost() > PlayerData.instance.premium)
		{
			itemList.buyButton.interactable = false;
			itemList.premiumText.color = Color.red;
		}
		else
		{
			itemList.premiumText.color = Color.black;
		}
	}

    public void Buy(Consumable c)
    {
        
        additional = 0;

       
        
            for (float o = 0f; o <= PlayerData.instance.pudm[c.identifier]; o += 2f)
            {
                int multi = (int)o;
                if (multi > 0) { multi = multi / 2; }
                additional += c.GetPrice() * multi;

                
            }
        
        actualprice = c.GetPrice() + additional;
        PlayerData.instance.coins -= actualprice;
		PlayerData.instance.premium -= c.GetPremiumCost();
		PlayerData.instance.Add(c.GetConsumableType());
        
        PlayerData.instance.Save();

#if UNITY_ANALYTICS // Using Analytics Standard Events v0.3.0
        var transactionId = System.Guid.NewGuid().ToString();
        var transactionContext = "store";
        var level = PlayerData.instance.rank.ToString();
        var itemId = c.GetConsumableName();
        var itemType = "consumable";
        var itemQty = 1;

        AnalyticsEvent.ItemAcquired(
            AcquisitionType.Soft,
            transactionContext,
            itemQty,
            itemId,
            itemType,
            level,
            transactionId
        );
        
        if (c.GetPrice() > 0)
        {
            AnalyticsEvent.ItemSpent(
                AcquisitionType.Soft, // Currency type
                transactionContext,
                c.GetPrice(),
                itemId,
                PlayerData.instance.coins, // Balance
                itemType,
                level,
                transactionId
            );
        }

        if (c.GetPremiumCost() > 0)
        {
            AnalyticsEvent.ItemSpent(
                AcquisitionType.Premium, // Currency type
                transactionContext,
                c.GetPremiumCost(),
                itemId,
                PlayerData.instance.premium, // Balance
                itemType,
                level,
                transactionId
            );
        }
#endif

        Refresh();
    }
    public void Upgrade(Consumable c)
    {
        
        additional = 0;
       
       
       
            for (float o = 0f; o <= PlayerData.instance.pudm[c.identifier]; o += 2f)
            {
                int multi = (int)o;
                if (multi > 0) { multi = multi / 2; }
                additional += c.GetPrice() * multi;
               
            }
        
        actualprice = c.GetPrice() + additional;
        PlayerData.instance.coins -= actualprice;
        PlayerData.instance.premium -= c.GetPremiumCost();
        PlayerData.instance.Add(c.GetConsumableType());
        PlayerData.instance.pudm[c.identifier] += 2f;
        PlayerData.instance.Save();


        Refresh();
    }
}
