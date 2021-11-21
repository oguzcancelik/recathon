import React from 'react';
import Card from '@material-ui/core/Card';
import CardContent from '@material-ui/core/CardContent';
import CardMedia from '@material-ui/core/CardMedia';
import Typography from '@material-ui/core/Typography';
import {black, white} from "../../infrastructure/constants/color-constants";

const PlaylistCard = props => {

    const color = props.playlist && props.playlist.color && props.playlist.color !== white
        ? white
        : black;

    return props.playlist
        ? (
            <Card id={props.playlist.id}
                  onClick={() => {
                      if (props.handleSelect) {
                          props.handleSelect(props.playlist.id);
                      }
                  }}
                  style={{
                      marginTop: '5%',
                      borderWidth: 8,
                      borderStyle: 'solid',
                      background: props.playlist.color,
                      borderColor: props.playlist.color,
                      color: color,
                      filter: props.playlist.disable ? "blur(2px)" : null
                  }}
            >
                <CardMedia style={{height: 0, paddingTop: '80%'}} image={props.playlist.imagePath}/>

                <CardContent>
                    <Typography gutterBottom variant="h5" component="h2" style={{textAlign: "center"}}>
                        {props.playlist.name}
                    </Typography>
                    {props.playlist.trackCount > 0
                        ?
                        <Typography variant="body2" component="p" style={{textAlign: "center"}}>
                            {`Number of Tracks: ${props.playlist.trackCount}`}
                        </Typography>
                        : null
                    }
                </CardContent>
            </Card>
        )
        : null;
}

export default PlaylistCard;
