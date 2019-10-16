var ParticleA : GameObject;
//var bulletHole : GameObject;

function Update () {

var hit : RaycastHit;
// Use Screen.height because many functions (like this one) start in the bottom left of the screen, while MousePosition starts in the top left
var ray : Ray = Camera.main.ScreenPointToRay (Input.mousePosition);

    if (Input.GetMouseButtonDown(0)) 
        {
        if (Physics.Raycast (ray, hit, 200)) 
           {
           var newparA = Instantiate(ParticleA, hit.point, Quaternion.identity);
 		   //var hitRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
           //Instantiate(bulletHole, hit.point, hitRotation);
 		   Destroy(newparA, 0.5);
		   }
        }
    }
