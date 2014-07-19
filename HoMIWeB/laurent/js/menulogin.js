	var ListeServeurs = [];
	var SiteActuel = 0;
	

// enregistre les informations d'un site
function ClickBouton(){

	zones.removeAll();
	macros.removeAll();
	
	// Lancement de l'application 
	 if (!window.sessionStorage){
        return;
	 }
	// alert("Lancement de l'application"+ SiteActuel);
	sessionStorage.SiteActuel= SiteActuel;

	ServerName =  ListeServeurs()[SiteActuel].SERVER; 
	PortActuel = ListeServeurs()[SiteActuel].Port;
	AdresseIPActuel = ListeServeurs()[SiteActuel].AdresseIP;
	IDSERVERActuel = ListeServeurs()[SiteActuel].IDSERVER;
	ConnectServer();

}  


// stocke tous les informations de site dans un tableau
function ReadSites(){
	var key;
	// console.log("Passage par ReadSites");
	// Vide le tableau contenant la definition des sites
 	ListeServeurs.splice(0, ListeServeurs.length);
	$('#ListeServeurs').empty();
	
	if(typeof(Storage)!=="undifined"){
		var Cpt = localStorage.length;
	}	
	
	if  (Cpt)  {
	// Redessine toutes les icones
		for (i=0;i<Cpt;i++) {
			key = localStorage.key(i);
			console.log("Valeurs sauvegardées : "+key);
		//	ListeServeurs.push(key);
			InfosServeur = JSON.parse(localStorage.getItem(key));
			console.log("Valeurs Site: "+i+" "+InfosServeur.SERVER);			
			ListeServeurs.push(InfosServeur);
		//	$('#ListeServeurs').append('<div><img src="Images/icones/Home_128.png" onclick="AfficheSite(' +  i +')" /><span>'+ InfosServeur.SERVER + '</span></div>');
			if  (InfosServeur.ServerDef) {
				AfficheSite(i);
				ClickBouton();
			} 
		}
	}
	
}	

// Affiche les infos du site selectionné
function AfficheSite(IndexSite){
//	alert("Passage par la fonction AfficheSite avec le parametre : "+IndexSite +" / "+ ListeServeurs()[IndexSite].SERVER);
	document.getElementById('ServerName').value = ListeServeurs()[IndexSite].SERVER;
	document.getElementById('ServerID').value = ListeServeurs()[IndexSite].IDSERVER;
	document.getElementById('ServerIP').value = ListeServeurs()[IndexSite].AdresseIP;
	document.getElementById('ServerPort').value = ListeServeurs()[IndexSite].Port;
	$("#ServerDef").prop('checked',ListeServeurs()[IndexSite].ServerDef);

	SiteActuel = IndexSite;
}

// Affiche les infos du site selectionné
function SupprimeSite(){
	if (ListeServeurs().length) {
		//alert("Passage par la fonction SupprimeSite avec le parametre : "+SiteActuel + " - " +ListeServeurs()[SiteActuel].NomSite)  ;	
		localStorage.removeItem(ListeServeurs()[SiteActuel].NomSite);
		ListeServeurs.splice(SiteActuel,1);	
		ReadSites();
	}
	
}

// enregistre les informations d'un site
function AjouterSite(){
	
	var InfosServeur = new Object();
	var storage = window.localStorage;

	// alert("Passage par la fonction AjouterSite avec le parametre : ")
	if(typeof(Storage)!=="undifined"){
		var Cpt = storage.length;
	}
	
	
	var ServerName = document.getElementById('ServerName').value;
	if (!ServerName=="") {	
		var NomSite = "Site"+"::"+ ListeServeurs.length ;
	
		InfosServeur.NomSite    = NomSite;
		InfosServeur.SERVER    = document.getElementById('ServerName').value;
		InfosServeur.IDSERVER  = document.getElementById('ServerID').value;
		InfosServeur.AdresseIP = document.getElementById('ServerIP').value;
		InfosServeur.Port      = document.getElementById('ServerPort').value;
		InfosServeur.ServerDef = $('#ServerDef').is(":checked");

		localStorage.setItem(NomSite, JSON.stringify(InfosServeur));	
		ReadSites();  	
	}
	else {
		alert('Veuillez saisir les informations ! ');
	}
}  