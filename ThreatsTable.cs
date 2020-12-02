using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ThreatsList
{
    class ThreatsTable
    {
        public string Number { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public string Confidentiality { get; set; }
        public string Integrity { get; set; }
        public string Availability { get; set; }

        public ThreatsTable(string number, string name, string description, string source, string target, string confidentiality, string integrity, string availability)
        {
            Number = number;
            Name = name;
            Description = description;
            Source = source;
            Target = target;
            Confidentiality = confidentiality;
            Integrity = integrity;
            Availability = availability;
        }

        public override string ToString()
        {
            return $"   Идендификатор угрозы: {this.Number}\n" +
                   $"   Наименование угрозы: {this.Name}\n" +
                   $"   Описание угрозы: {this.Description}\n" +
                   $"   Источник угрозы: {this.Source}\n" +
                   $"   Объект воздействия угрозы: {this.Target}\n" +
                   $"   Нарушение конфиденциальности: {(this.Confidentiality == "1" ? "Да" : "Нет")}\n" +
                   $"   Нарушение целостности: {(this.Integrity == "1" ? "Да" : "Нет")}\n" +
                   $"   Нарушение доступности: {(this.Availability == "1" ? "Да" : "Нет")}";
        }

        public override bool Equals(object obj)
        {
            if (obj is ThreatsTable)
            {
                ThreatsTable o = obj as ThreatsTable;
                return  this.Number == o.Number && 
                        this.Name == o.Name && 
                        this.Description == o.Description &&
                        this.Source == o.Source &&
                        this.Target == o.Target &&
                        this.Confidentiality == o.Confidentiality &&
                        this.Integrity == o.Integrity &&
                        this.Availability == o.Availability;
            }
            else
            {
                return true;
            }
        }
    }
}
