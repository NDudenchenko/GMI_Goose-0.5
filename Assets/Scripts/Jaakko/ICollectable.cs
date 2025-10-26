using UnityEngine;

namespace AG3958
{
	public interface ICollectable
	{
		enum CollectableType { Points, Health, Mana, Major }
		CollectableType CType { get; }
		float CValue { get; }

		void OnTriggerEnter2D(Collider2D collision);
		void CollectObject();
	}

}