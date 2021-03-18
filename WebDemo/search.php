<meta charset="utf-8" />
<link href='css\style.css' rel='stylesheet' type='text/css'>
<?php
	//資料庫設定
	//資料庫位置
	$db_server = "localhost";
	//資料庫名稱
	$db_name = "pcretrieval";
	//資料庫管理者帳號
	$db_user = "pcretrieval";
	//資料庫管理者密碼
	$db_passwd = "B9147DB746426832";

	//對資料庫連線
	$con=mysqli_connect($db_server, $db_user, $db_passwd, $db_name);
	if(!@$con)
		die("無法對資料庫連線");

	//資料庫連線採UTF-8
	mysqli_query($con,"SET NAMES utf8");
	$tabletitle=array("<th class='thCont'>Rank</th>",
										"<th class='thCont'>Thumbnail</th>",
										"<th class='thCont'>Similarness</th>",
										"<th class='thCont'>mid</th>",
										"<th class='thCont'>obj file</th>",
										"<th class='thCont'>Source</th>");
	$sql="SELECT `ranking`,
CONCAT('<img width=150 src=data/db/',`mod4`,'/',SubStr(`mid`,1,2),'/',substr(`mid`,3,2),'/',`mid`,'.jpg>'),
`distance`,
CONCAT('<a href=javascript:DoRequery(',`id`-1,')>',`mid`,'</a>'),
CONCAT('<a href=data/db/',`mod4`,'/',SubStr(`mid`,1,2),'/',substr(`mid`,3,2),'/',`mid`,'.obj00 target=_blank>obj00</a>'),
CONCAT('<a href=https://3dwarehouse.sketchup.com/model.html?id=',`mid`,' target=_blank>Google 3D Warehouse</a>')
FROM `query_result` WHERE 1";
	$result = mysqli_query($con,$sql) or die('輸入的資料有誤!');
	
	
	echo "<TABLE rules=all ALIGN='CENTER' class='tabCont'>";
	// 顯示欄位內容
	for($i=0;$i<6;$i++)
		{
			echo "$tabletitle[$i]";
		}
	for($j=0;$j<mysqli_num_rows($result);$j++)
	{
		echo "<TR class='trCont'>";
		for ($k=0;$k<mysqli_num_fields($result);$k++)
		{
			echo "<TD class='tdCont' align=center>".mysqli_result($result,$j,$k)."</TD>";
		}
		echo "</TR>";
	}
	echo "</TABLE>";
		
	
	function mysqli_result($result, $row, $field = 0) 
	{
    // Adjust the result pointer to that specific row
    $result->data_seek($row);
    // Fetch result array
    $data = $result->fetch_array();
 
    return $data[$field];
}
?>