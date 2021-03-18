<html>
<head>
<meta charset="utf-8" />

</head>
<?php
if(isset($_FILES["file"]["name"]))
{
	if($_FILES["file"]["error"]>0)
		echo $_FILES["file"]["error"];
	else
	{
		$output_dir = "program/upload/";
		$type=substr($_FILES["file"]["name"],-4);
		if($type==".xyz" || $type==".obj")
		{
			move_uploaded_file($_FILES["file"]["tmp_name"],$output_dir."tmp".$type);
			exec("cd program && ./run.sh "."./upload/tmp".$type,$out,$return);
			$num=count($out);
			if($return == 1)
			{
				echo $out[$num-3]."<br>";
				echo $out[$num-2]."<br>";
				echo $out[$num-1]."<br>";
			}
			echo "<input type='hidden' id='return_Value' value='".$return."' />";
		}
	}
}
?>