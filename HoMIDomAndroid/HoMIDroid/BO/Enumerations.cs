using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace HoMIDroid.BO
{

    public enum DeviceType
    {
        OnOff,
        Dim,
        Numeric,
        Other
    }

    public enum DeviceCategory
    {
        Light,
        Switch,
        Multimedia,
        Meteo,
        Energy,
        Other
    }
    public enum DeviceFamily
    {
        APPAREIL = 1,// 'modules pour diriger un appareil  ON/OFF
        AUDIO = 2,
        BAROMETRE = 3,// 'pour stocker les valeur issu d'un barometre meteo ou web
        BATTERIE = 4,
        COMPTEUR = 5,// 'compteur DS2423, RFXPower...
        CONTACT = 6,// 'detecteur de contact : switch 1-wire
        DETECTEUR = 7,// 'tous detecteurs : mouvement, obscurite...
        DIRECTIONVENT = 8,
        ENERGIEINSTANTANEE = 9,
        ENERGIETOTALE = 10,
        FREEBOX = 11,
        GENERIQUEBOOLEEN = 12,
        GENERIQUESTRING = 13,
        GENERIQUEVALUE = 14,
        HUMIDITE = 15,
        LAMPE = 16,
        METEO = 17,
        MULTIMEDIA = 18,
        PLUIECOURANT = 19,
        PLUIETOTAL = 20,
        SWITCH = 21,
        TELECOMMANDE = 22,
        TEMPERATURE = 23,
        TEMPERATURECONSIGNE = 24,
        UV = 25,
        VITESSEVENT = 26,
        VOLET = 27
    }


    public enum DisplayType
    {
        Boolean,
        Temperature,
        Integer,
        Numeric,
        Percentage,
        NoValue,
        Text
    }
}