public class Position
{
    public int  R {get; }
    public int  C {get; }


    public Position (int r, int c){

        R = r;
        C = c;

    }

    public override int GetHashCode(){
        return 8 * R + C;
    }

    public override bool Equals(object a){

        if (a is Position other){

            return R == other.R && C == other.C;
        }

        return false;
    }

    
}
