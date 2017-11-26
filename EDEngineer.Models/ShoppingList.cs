﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EDEngineer.Models
{
    public class ShoppingList : IEnumerable<Blueprint>
    {
        private readonly ILanguage languages;
        private readonly List<Blueprint> blueprints;

        public ShoppingList(List<Blueprint> blueprints, ILanguage languages)
        {
            this.blueprints = blueprints;
            this.languages = languages;
        }

        public List<Blueprint> List => this.ToList();

        public List<Tuple<Blueprint, int>> Composition
            => blueprints.Where(b => b.ShoppingListCount > 0)
                         .Select(b => Tuple.Create(b, b.ShoppingListCount)).ToList();

        public IEnumerator<Blueprint> GetEnumerator()
        {
            var ingredients = blueprints
                .SelectMany(b => Enumerable.Repeat(b, b.ShoppingListCount))
                .SelectMany(b => b.Ingredients)
                .ToList();

            if (!ingredients.Any())
            {
                yield break;
            }

            var composition = ingredients.GroupBy(i => i.Entry.Data.Name)
                                         .Select(
                                             i =>
                                                 new BlueprintIngredient(i.First().Entry,
                                                     i.Sum(c => c.Size)))
                                         .OrderBy(i => i.Entry.Data.Kind)
                                         .ThenBy(i => languages.Translate(i.Entry.Data.Name))
                                         .ToList();

            var metaBlueprint = new Blueprint(languages, "", "Shopping List", -1, composition,
                new string[0]);

            yield return metaBlueprint;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}