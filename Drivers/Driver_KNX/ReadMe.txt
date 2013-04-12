Driver KNX/EIBD pour HoMIDom
----------------------------
Permet d'interragir avec un BUS KNX à travers une passerelle IP/KNX (EIBd)

Les objets KNX doivent être configurés dans le fichier ~\Drivers\KNX\conf.xml

<?xml version="1.0" encoding="utf-8" ?>
<KNXObjects>
  <KNXObject id="ecl_bureau" gad="1/5/10" name="Lampe principale bureau" type="1.001" flags="cwrtu"/>
  <KNXObject id="ecl_bureau_ie" gad="1/5/11" name="Lampe principale salon" type="1.001"/>
</KNXObjects>

L'ID des Objets KNX correspond à l'adresse1 des Devices HoMIDom


Types de Devices pris en charge:
-----------------------------------

* ListeDevices.SWITCH

Types de Commandes pris en charge:
-------------------------------------

* ON
* OFF