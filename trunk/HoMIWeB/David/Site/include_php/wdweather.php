<?php

class WdWeather
{
    var $markup_contents = '([^<]+)';

    function getWeather($city_code, $days) {
        $xml = @file_get_contents('http://xml.weather.com/weather/local/' . $city_code . '?cc=*&unit=m&dayf=' . $days); 
        if (!$xml) {return;}
        return $this->parseDays($xml);
    }
    
    function getWeather2($city_code, $days) {
        $xml = @file_get_contents('http://xml.weather.com/weather/local/' . $city_code . '?cc=*&unit=m&dayf=' . $days);
        if (!$xml) {return;}
        return $this->parseToday($xml);
    }

    function parseDays($xml) {
        $days = array();
 	    $parts = preg_split('#<day d=[^>]+>#', $xml);
        array_shift($parts);
        foreach ($parts as $xml2) {$days[] = $this->parseDay($xml2);}
        //recup du jour/date dans le titre du day
        $x = strpos($xml,"<day d=",0);//recherche le premier jour
        while ($x>0) {
	        $temp=substr($xml,$x,(strpos($xml,">",$x)-$x));
	        preg_match('<day d="([0-9])" t="([a-zA-Z]*)" dt="([a-zA-Z0-9 ]*)">',$temp,$temp2);
	        $days[$temp2[1]]["jour"]=$this->jourstoFR($temp2[2]);
	        $days[$temp2[1]]["date"]=date("j",strtotime($temp2[3]))." ".$this->moistofr(date("m",strtotime($temp2[3])));
	        $x = strpos($xml,"<day d=",$x+1); //recherche le prochain jour
        }        
        return $days;
    }
    
    function parsetoday($xml) {
        $days = array();
        $parts = preg_split('#<cc>#', $xml);
        array_shift($parts);
        foreach ($parts as $xml) {$cc[] = $this->parseCC($xml);}
        return $cc;
    }

    function parseDay($xml) {
        $mc = $this->markup_contents;
        preg_match
        (
            '#' .
            '<hi>' . $mc . '</hi>\s*' .
            '<low>' . $mc . '</low>\s*' .
            '<sunr>' . $mc . '</sunr>\s*' .
            '<suns>' . $mc . '</suns>\s*' .
            '<part p="d">(.*)</part>\s*' .
            '<part p="n">(.*)</part>' .
            '#ms',
            $xml, $matches
        );
        array_shift($matches);         
        $matches[2] = $this->to24H($matches[2]);
        $matches[3] = $this->to24H($matches[3]);
        $matches[4] = $this->parseDayPeriod($matches[4]);
        $matches[5] = $this->parseDayPeriod($matches[5]);
        return array_combine(array('hi', 'low', 'sunr', 'suns', 'day', 'night'), $matches);
    }
   
    function parseDayPeriod($xml) {
        $mc = $this->markup_contents;
        preg_match
        (
            '#' .
            '<icon>' . $mc . '</icon>\s*' .
            '<t>' . $mc . '</t>\s*' .
            '<wind>(.*)</wind>\s*' .
            '<bt>' . $mc . '</bt>\s*' .
            '<ppcp>' . $mc . '</ppcp>\s*' .
            '<hmid>' . $mc . '</hmid>' .
            '#ms',
            $xml, $matches
        );
        array_shift($matches);
        $matches[2] = $this->parseWind($matches[2]);
        return array_combine(array('icon', 't', 'wind', 'bt', 'ppcp', 'hmid'), $matches);
    }
 
    function parseCC($xml) {
        $mc = $this->markup_contents;
        preg_match
        (
            '#' .
            '<lsup>' . $mc . '</lsup>\s*' .
            '<obst>' . $mc . '</obst>\s*' .
            '<tmp>' . $mc . '</tmp>\s*' .
            '<flik>' . $mc . '</flik>\s*' .
            '<t>' . $mc . '</t>\s*' .
            '<icon>' . $mc . '</icon>\s*' .
            '<bar>(.*)</bar>\s*' .
            '<wind>(.*)</wind>\s*' .
            '<hmid>' . $mc . '</hmid>\s*' .
            '<vis>' . $mc . '</vis>\s*' .
            '<uv>(.*)</uv>\s*' .
            '<dewp>' . $mc . '</dewp>\s*' .
            '<moon>(.*)</moon>\s*' .
            '#ms',
            $xml, $matches
        );
        array_shift($matches);             
        # parse
        $matches[6] = $this->parseBar($matches[6]);
        $matches[7] = $this->parseWind($matches[7]);
        $matches[10] = $this->parseUV($matches[10]);
        $matches[12] = $this->parseMoon($matches[12]);
        return array_combine(array('lsup', 'obst', 'tmp', 'flik', 't', 'icon', 'bar', 'wind', 'hmid', 'vis', 'uv', 'dewp', 'moon'), $matches);
    }
    
    function parseWind($xml) {
        $mc = $this->markup_contents;
        preg_match
        (
            '#' .
            '<s>' . $mc . '</s>\s*' .
            '<gust>' . $mc . '</gust>\s*' .
            '<d>' . $mc . '</d>\s*' .
            '<t>' . $mc . '</t\s*' .
            '#ms',
            $xml, $matches
        );
        array_shift($matches);
        return array_combine(array('s', 'gust', 'd', 't'), $matches);
    }
    
    function parseBar($xml) {
        $mc = $this->markup_contents;
        preg_match
        (
            '#' .
            '<r>' . $mc . '</r>\s*' .
            '<d>' . $mc . '</d>\s*' .
            '#ms',
            $xml, $matches
        );
        array_shift($matches);
        return array_combine(array('r', 'd'), $matches);
    }
    
    function parseUv($xml) {
        $mc = $this->markup_contents;
        preg_match
        (
            '#' .
            '<i>' . $mc . '</i>\s*' .
            '<t>' . $mc . '</t\s*' .
            '#ms',
            $xml, $matches
        );
        array_shift($matches);
        return array_combine(array('i', 't'), $matches);
    }
    
    function parseMoon($xml) {
        $mc = $this->markup_contents;
        preg_match
        (
            '#' .
            '<icon>' . $mc . '</icon>\s*' .
            '<t>' . $mc . '</t\s*' .
            '#ms',
            $xml, $matches
        );
        array_shift($matches);
        return array_combine(array('icon', 't'), $matches);
    }
 
    static function to24H($time) {
        preg_match('#(\d+)\:(\d+)\s+(AM|PM)?#', $time, $matches);
        if ($matches[3] == 'PM') {$matches[1] += 12;}
        return $matches[1] . ':' . $matches[2];
    }
    static function tempstoFR($temps) {
	    $temps2=strtolower(str_replace(array(" ","/"),"",$temps));
	    switch ($temps2) {
		    case "afewclouds":return "Quelques nuages";break;
			case "amcloudspmsun":return "Nuageux dans la matinée / Soleil dans l'après-midi";break;
			case "amcloudspmsunwind":return "Nuageux dans la matinée / Soleil dans l'après-midi / Vent";break;
			case "amdrizzle":return "Bruine dans la matinée";break;
			case "amfogpmsun":return "Brouillard dans la matinée / Soleil dans l'après-midi";break;
			case "amfogpmclouds":return "Brouillard dans la matinée / Nuageux dans l'après-midi";break;
			case "amlightrain":return "Légère pluie dans la matinée";break;
			case "amlightrainwind":return "Légère pluie dans la matinée / Vent";break;
			case "amlightsnow":return "Légères chutes de neige dans la matinée";break;
			case "amlightsnowwind":return "Légères chutes de neige dans la matinée / Vent";break;
			case "amlightwintrymix":return "Légères précipitations hivernales dans la matinée";break;
			case "amrain":return "Pluie dans la matinée";break;
			case "amrainice":return "Pluie dans la matinée / Glace";break;
			case "amrainsnow":return "Pluie dans la matinée / Neige";break;
			case "amrainsnowwind":return "Pluie dans la matinée / Neige / Vent";break;
			case "amrainsnowshowers":return "Pluie / Chutes de neige dans la matinée";break;
			case "amrainwind":return "Pluie dans la matinée / Vent";break;
			case "amshowers":return "Averses dans la matinée";break;
			case "amshowerswind":return "Averses dans la matinée / Vent";break;
			case "amsnow":return "Neige dans la matinée";break;
			case "amsnowshowers":return "Chutes de neige dans la matinée";break;
			case "amsnowshowerswind":return "Chutes de neige dans la matinée / Vent";break;
			case "amt-storms":return "Orages dans la matinée";break;
			case "amwintrymix":return "Précipitations hivernales dans la matinée";break;
			case "blowingsandandwindy":return "Rafales de sable et Vent";break;
			case "blowingsnow":return "Rafales de neige";break;
			case "clear":return "Ciel dégagé";break;
			case "cloudsearlyclearinglate":return "Nuageux d'abord / éclaircie ensuite";break;
			case "cloudy":return "Nuageux";break;
			case "cloudywind":return "Nuageux / Vent";break;
			case "cloudyandwindy":return "Nuageux et Venteux";break;
			case "driftingsnow":return "Rafales de neige";break;
			case "drizzle":return "Bruine";break;
			case "drizzlefog":return "Bruine / Brouillard";break;
			case "fair":return "Beau";break;
			case "fairandwindy":return "Beau et Venteux";break;
			case "fewshowers":return "Quelques averses";break;
			case "fewshowerswind":return "Quelques averses / Vent";break;
			case "fewsnowshowers":return "Quelques chutes de neige";break;
			case "fewsnowshowerswind":return "Quelques chutes de neige / Vent";break;
			case "flurries":return "Chutes de neige fondante";break;
			case "flurrieswind":return "Chutes de neige fondante / Vent";break;
			case "fog":return "Brouillard";break;
			case "fogclouds":return "Brouillard / Ciel se couvrant";break;
			case "foggy":return "Brouillard";break;
			case "foglate":return "Brouillard";break;
			case "freezingrain":return "Pluie verglaçante";break;
			case "haze":return "Légère brume";break;
			case "heavydrizzle":return "Forte bruine";break;
			case "heavyrain":return "Forte pluie";break;
			case "heavyrainwind":return "Forte pluie / Vent";break;
			case "heavyrainshower":return "Fortes averses";break;
			case "heavyrainshowerandwindy":return "Fortes averses et Vent";break;
			case "heavysnow":return "Fortes chutes de neige";break;
			case "heavyt-storm":return "Orage violent";break;
			case "heavyt-stormandwindy":return "Orage violent et Vent";break;
			case "heavyt-storms":return "Orages violents";break;
			case "heavyt-stormswind":return "Orages violents / Vent";break;
			case "hvyrainfreezingrain":return "Forte pluie / Pluie verglaçante";break;
			case "icetorain":return "Givre puis Pluie";break;
			case "isot-stormswind":return "Orages isolés / Vent";break;
			case "isolatedt-storms":return "Orages isolés";break;
			case "lightdrizzle":return "Légère bruine";break;
			case "lightfreezingdrizzle":return "Légère bruine verglaçante";break;
			case "lightfreezingrain":return "Légère pluie verglaçante";break;
			case "lightrain":return "Légère pluie";break;
			case "lightrainearly":return "Légère pluie";break;
			case "lightrainfog":return "Légère pluie / Brouillard";break;
			case "lightrainfreezingrain":return "Légère pluie / Pluie verglaçante";break;
			case "lightrainlate":return "Légère pluie tardive";break;
			case "lightrainwind":return "Légère pluie / Vent";break;
			case "lightrainandfog":return "Légère pluie et Brouillard";break;
			case "lightrainandfreezingrain":return "Légère pluie et Pluie verglaçante";break;
			case "lightrainandwindy":return "Légère pluie et Vent";break;
			case "lightrainshower":return "Légères averses";break;
			case "lightrainshowerandwindy":return "Légères averses et Vent";break;
			case "lightrainwiththunder":return "Légère pluie avec tonnerre";break;
			case "lightsnow":return "Légères chutes de neige";break;
			case "lightsnowwind":return "Légère neige / Vent";break;
			case "lightsnowandfog":return "Légère neige et Brouillard";break;
			case "lightsnowfog":return "Légère neige et Brouillard";break;
			case "lightsnowearly":return "Légères chutes de neige";break;
			case "lightsnowgrains":return "Légers granules de neige";break;
			case "lightsnowgrainsandfog":return "Légers granules de neige et Brouillard";break;
			case "lightsnowlate":return "Légères chutes de neige";break;
			case "lightsnowshower":return "Légères chutes de neige";break;
			case "lightsnowshowerwind":return "Légères chutes de neige / Vent";break;
			case "mist":return "Brume";break;
			case "mostlyclear":return "Ciel plutôt dégagé";break;
			case "mostlyclearwind":return "Ciel plutôt dégagé / Vent";break;
			case "mostlycloudy":return "Plutôt nuageux";break;
			case "mostlycloudywind":return "Plutôt nuageux / Vent";break;
			case "mostlycloudyandwindy":return "Plutôt nuageux et Venteux";break;
			case "mostlysunny":return "Plutôt ensoleillé";break;
			case "mostlysunnywind":return "Plutôt ensoleillé / Vent";break;
			case "partlycloudy":return "Passages nuageux";break;
			case "partlycloudywind":return "Passages nuageux / Vent";break;
			case "partlycloudyandwindy":return "Passages nuageux et Vent";break;
			case "patchesoffog":return "Bancs de brouillard";break;
			case "pmdrizzle":return "Bruine dans l'après-midi";break;
			case "pmfog":return "Brouillard dans l'après-midi";break;
			case "pmlightrain":return "Légère pluie dans l'après-midi";break;
			case "pmlightrainice":return "Légère pluie dans l'après-midi / Verglas";break;
			case "pmlightrainwind":return "Légère pluie dans l'après-midi / Vent";break;
			case "pmlightsnow":return "Légères chutes de neige dans l'après-midi";break;
			case "pmlightsnowwind":return "Légères chutes de neige dans l'après-midi / Vent";break;
			case "pmrain":return "Pluie dans l'après-midi";break;
			case "pmrainsnow":return "Pluie / Neige dans l'après-midi";break;
			case "pmrainsnowwind":return "Pluie / Neige / Vent dans l'après-midi";break;
			case "pmrainsnowshowers":return "Pluie / Chutes de neige dans l'après-midi";break;
			case "pmshowers":return "Averses dans l'après-midi";break;
			case "pmshowerswind":return "Averses / Vent dans l'après-midi";break;
			case "pmsnow":return "Chutes de neige dans l'après-midi";break;
			case "pmsnowwind":return "Chutes de neige dans l'après-midi / Vent";break;
			case "pmsnowshowers":return "Chutes de neige dans l'après-midi";break;
			case "pmsnowshowerswind":return "Chutes de neige dans l'après-midi / Vent";break;
			case "pmt-showers":return "Averses orageuses dans l'après-midi";break;
			case "pmt-storms":return "Orages dans l'après-midi";break;
			case "pmwintrymix":return "Précipitations hivernales dans l'après-midi";break;
			case "rain":return "Pluie";break;
			case "rainfreezingrain":return "Pluie / Pluie verglaÃ§ante";break;
			case "rainsleet":return "Pluie / Granules de glace";break;
			case "rainsnow":return "Pluie / Neige";break;
			case "rainsnowlate":return "Pluie / Neige tardive";break;
			case "rainsnowwind":return "Pluie / Neige / Vent";break;
			case "rainsnowshowers":return "Pluie / Chutes de neige";break;
			case "rainsnowshowerswind":return "Pluie / Chutes de neige / Vent";break;
			case "rainsnowshowerslate":return "Pluie / Chutes de neige";break;
			case "rainthunder":return "Pluie / Tonnerre";break;
			case "rainthunderwind":return "Pluie / Tonnerre / Vent";break;
			case "rainwind":return "Pluie / Vent";break;
			case "rainandsleet":return "Pluie et Granules de glace";break;
			case "rainandsnow":return "Pluie et Neige";break;
			case "rainshower":return "Averses";break;
			case "rainshowerandwindy":return "Averses et Vent";break;
			case "raintosnow":return "Pluie devenant neige";break;
			case "raintosnowwind":return "Pluie devenant neige / Vent";break;
			case "scatteredflurries":return "Chutes de neige fondante éparses";break;
			case "scatteredflurrieswind":return "Chutes de neige fondante éparses / Vent";break;
			case "scatteredshowers":return "Averses éparses";break;
			case "scatteredshowerswind":return "Averses éparses / Vent";break;
			case "scatteredsnowshowers":return "Alternance de chutes de neige";break;
			case "scatteredsnowshowerswind":return "Chutes de neige éparses / Vent";break;
			case "scatteredstrongstorms":return "Orages violents épars";break;
			case "scatteredt-storms":return "Orages épars";break;
			case "scatteredt-stormswind":return "Orages épars / Vent";break;
			case "shallowfog":return "Brouillard";break;
			case "showers":return "Averses";break;
			case "showerswind":return "Averses / Vent";break;
			case "showerswindlate":return "Averses / Vent";break;
			case "showersearly":return "Averses";break;
			case "showersinthevicinity":return "Averses dans le voisinage";break;
			case "showerslate":return "Averses";break;
			case "sleet":return "Granules de glace ";break;
			case "smoke":return "Fumée";break;
			case "snow":return "Neige";break;
			case "snowwind":return "Neige / Vent";break;
			case "snowandfog":return "Neige et Brouillard";break;
			case "snowandicetorain":return "Neige et Glace puis Pluie";break;
			case "snowgrains":return "Granules de neige";break;
			case "snowshower":return "Chutes de neige";break;
			case "snowshowerwind":return "Chutes de neige / Vent";break;
			case "snowshowerwindearly":return "Chutes de neige / Vent";break;
			case "snowshowersearly":return "Chutes de neige";break;
			case "snowshowerslate":return "Chutes de neige tardive";break;
			case "snowtoice":return "Neige puis Verglas";break;
			case "snowtoicewind":return "Neige se transformant en glace / Vent";break;
			case "snowtorain":return "Neige puis Pluie";break;
			case "snowtorainwind":return "Neige puis Pluie / Vent";break;
			case "snowtowintrymix":return "Neige puis Précipitations hivernales";break;
			case "sprinkles":return "Averses";break;
			case "strongstorms":return "Orages violents";break;
			case "strongstormswind":return "Orages violents / Vent";break;
			case "sunny":return "Ensoleillé";break;
			case "sunnywind":return "Ensoleillé / Vent";break;
			case "sunnyandwindy":return "Ensoleillé et Venteux";break;
			case "t-showers":return "Averses orageuses";break;
			case "t-showerswind":return "Averses orageuses / Vent";break;
			case "t-storm":return "Orage";break;
			case "t-storms":return "Orages";break;
			case "t-stormswind":return "Orages / Vent";break;
			case "t-stormsearly":return "Orages";break;
			case "t-stormslate":return "Orages";break;
			case "thunder":return "Tonnerre";break;
			case "thunderandwintrymix":return "Tonnerre et Précipitations hivernales";break;
			case "thunderinthevicinity":return "Tonnerre dans le voisinage";break;
			case "unknownprecip":return "Précipitations";break;
			case "widespreaddust":return "Brume sèche";break;
			case "wintrymix":return "Précipitations hivernales";break;
			case "wintrymixwind":return "Précipitations hivernales / Vent";break;
			case "wintrymixtosnow":return "Précipitations hivernales puis Neige";break;
		    default : return $temps;break;
	    }
    }
    function jourstoFR($texte) {
        $jourfr["monday"]="Lundi";
	    $jourfr["tuesday"]="Mardi";
	    $jourfr["wednesday"]="Mercredi";
	    $jourfr["thursday"]="Jeudi";
	    $jourfr["friday"]="Vendredi";
	    $jourfr["saturday"]="Samedi";
	    $jourfr["sunday"]="Dimanche";
	    if(array_key_exists(strtolower($texte),$jourfr)) {return $jourfr[strtolower($texte)];} else {return $texte;}
    }
    
    function moistoFR($nb) {
	    $nb=(int)$nb;
        $moisfr[1]="Janvier";
	    $moisfr[2]="Février";
	    $moisfr[3]="Mars";
	    $moisfr[4]="Avril";
	    $moisfr[5]="Mai";
	    $moisfr[6]="Juin";
	    $moisfr[7]="Juillet";
	    $moisfr[8]="Aout";
	    $moisfr[9]="Septembre";
	    $moisfr[10]="Octobre";
	    $moisfr[11]="Novembre";
	    $moisfr[12]="Decembre";
	    return $moisfr[$nb];
    }
}

?>