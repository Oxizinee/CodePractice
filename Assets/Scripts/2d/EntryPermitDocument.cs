using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

    public class EntryPermitDocument : Document
    {
        [SerializeField] private TextMeshPro _nameText, _passportNumberText, _expirationDateText;
        public EntryPermitDocument(CitizenProfile citezenProfile) : base(citezenProfile)
        {
            _nameText.text = CitezenProfile.Name;
            _passportNumberText.text = CitezenProfile.PassportNumber;
            _expirationDateText.text = CitezenProfile.ExpirationDate.ToShortDateString();
        }
        public override void SetTextOnDocument()
        {
            _nameText.text = CitezenProfile.Name;
            _passportNumberText.text = CitezenProfile.PassportNumber;
            _expirationDateText.text = CitezenProfile.ExpirationDate.ToShortDateString();
        }
    }
