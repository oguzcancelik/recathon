import React from 'react';
import Avatar from '@material-ui/core/Avatar';
import Button from '@material-ui/core/Button';
import Paper from '@material-ui/core/Paper';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import {makeStyles} from '@material-ui/core/styles';
import Logo from '../../images/logo.png'
import '../../css/index.css'
import * as api from "../../infrastructure/api/api-gateway";
import {defaultBackgroundColor, white} from "../../infrastructure/constants/color-constants";

const useStyles = makeStyles(theme => {
    return ({
        root: {
            height: '100vh',
        },
        image: {
            backgroundImage: 'url(https://source.unsplash.com/random?music)',
            backgroundRepeat: 'no-repeat',
            backgroundColor: theme.palette.type === 'light' ? theme.palette.grey[50] : theme.palette.grey[900],
            backgroundSize: 'cover',
            backgroundPosition: 'center',
        },
        paper: {
            margin: theme.spacing(8, 4),
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
        },
        avatar: {
            margin: theme.spacing(1),
            backgroundColor: theme.palette.secondary.main,
        },
        form: {
            width: '100%',
            marginTop: theme.spacing(1),
        },
        submit: {
            margin: theme.spacing(3, 0, 2),
        }
    });
});

const handleClick = () => {
    api.getLoginUrl()
        .then(res => {
            if (res.data) {
                window.location = res.data;
            }
        });
};

const Login = () => {
    const classes = useStyles();

    return (
        <Grid container component="main" className={classes.root}>
            <Grid item xs={false} sm={4} md={7} className={classes.image}/>
            <Grid item xs={12} sm={8} md={5} component={Paper} elevation={6} square
                  style={{backgroundColor: defaultBackgroundColor}}>
                <div className={classes.paper} style={{marginTop: '15%'}}>
                    <img style={{width: '40%', alignContent: "center"}} src={Logo} alt="login"/>
                    <Typography component="h1" variant="h5"
                                style={{fontFamily: 'Viga', color: 'white', marginTop: '2%'}}>
                        Please Login with Your Spotify Account
                    </Typography>
                    <Avatar alt="Remy Sharp"
                            src="https://developer.spotify.com/assets/branding-guidelines/icon3@2x.png"
                            style={{width: 60, height: 60, marginTop: '5%'}}
                            className={classes.large}
                            onClick={handleClick}/>
                    <Button onClick={handleClick}>
                        <span style={{color: white, fontSize: 25}}>Login</span>
                    </Button>
                </div>
            </Grid>
        </Grid>
    );
}

export default Login;

