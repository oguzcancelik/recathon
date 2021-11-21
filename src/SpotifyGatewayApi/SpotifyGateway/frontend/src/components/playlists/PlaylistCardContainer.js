import React, {Component} from 'react';
import Grid from '@material-ui/core/Grid';
import Button from '@material-ui/core/Button';
import {dropSelectedPlaylist, selectPlaylist} from "../../store/actions/playlistActions";
import {connect} from "react-redux";
import PlaylistCard from "./PlaylistCard";
import {Redirect} from "react-router-dom";
import {defaultColor, white} from "../../infrastructure/constants/color-constants";

class PlaylistCardContainer extends Component {

    state = {
        redirectPath: null
    };

    componentDidMount() {
        this.props.dropSelectedPlaylist();
    }

    componentWillUnmount() {
        if (this.state.redirectPath === null) {
            this.props.dropSelectedPlaylist();
        }
    }

    handleSelect = id => {
        this.props.selectPlaylist(id);
    };

    handleRedirect = () => {
        if (this.props.selectedPlaylistId !== null) {
            this.setState({
                redirectPath: "/recommend"
            });
        } else {
            this.setState({
                redirectPath: "/"
            });
        }
    };

    render() {
        return this.state.redirectPath !== null
            ? <Redirect to={this.state.redirectPath}/>
            : (
                <div>
                    <div style={{textAlign: 'center'}}>
                        <Button onClick={this.handleRedirect}
                                disabled={!this.props.selectedPlaylistId}
                                variant="contained"
                                style={{
                                    backgroundColor: defaultColor,
                                    marginTop: '2%',
                                    fontSize: '20px',
                                    color: white
                                }}>
                            {this.props.selectedPlaylistId ? "Recommend" : "Select a playlist"}
                        </Button>
                    </div>

                    <Grid container>
                        {
                            this.props.playlists === null
                                ? null
                                : this.props.playlists.map(playlist => {

                                    playlist.color = playlist.id === this.props.selectedPlaylistId
                                        ? defaultColor
                                        : white;

                                    playlist.disable = this.props.selectedPlaylistId && playlist.id !== this.props.selectedPlaylistId;

                                    return (
                                        <Grid item xs={6} sm={4} lg={3} xl={2} key={playlist.id}
                                              style={{padding: '30px'}}>
                                            <PlaylistCard playlist={playlist} handleSelect={this.handleSelect}/>
                                        </Grid>
                                    );
                                })
                        }
                    </Grid>
                </div>
            );
    }
}

const mapStateToProps = state => {
    return {
        selectedPlaylistId: state.playlist.selectedPlaylistId
    };
};

const mapDispatchToProps = dispatch => {
    return {
        selectPlaylist: playlistId => dispatch(selectPlaylist(playlistId)),
        dropSelectedPlaylist: () => dispatch(dropSelectedPlaylist())
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(PlaylistCardContainer);
